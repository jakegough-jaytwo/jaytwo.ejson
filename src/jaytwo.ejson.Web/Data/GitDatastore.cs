using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace jaytwo.ejson.Web.Data
{
    public class GitDatastore : IGitDatastore, IDatastore
    {
        private static readonly Random _random = new Random();
        private const string RemoteName = "origin";
        private readonly TimeSpan _cacheTtl = TimeSpan.FromSeconds(2);
        private readonly string _url;
        private readonly string _branchName;
        private readonly string _basePath;
        private readonly string _authorName;
        private readonly string _authorEmail;
        private readonly string _username;
        private readonly string _password;

        public GitDatastore(string url, string branchName, string basePath, string authorName, string authorEmail, string username, string password)
        {
            _url = url;
            _branchName = branchName;
            _basePath = basePath;
            _authorName = authorName;
            _authorEmail = authorEmail;
            _username = username;
            _password = password;
        }

        public IList<FileInfo> GetFiles(string searchPattern, bool recursive)
        {
            GitPull();

            IList<string> indexPaths;
            using (var repository = new Repository(_basePath))
            {
                indexPaths = repository.Index.Select(x => x.Path).ToList();
            }

            var searchOption = recursive
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            var fileInfos = new DirectoryInfo(_basePath).GetFiles(searchPattern, searchOption);

            var result = fileInfos.Where(x => indexPaths.Contains(Path.GetRelativePath(_basePath, x.FullName))).ToList();
            return result;
        }

        public IList<string> GetFiles(string pattern)
        {
            return GetFiles(pattern, true)
                .Select(x => Path.GetRelativePath(_basePath, x.FullName))
                .ToList();
        }

        public void SaveContent(string key, Stream stream)
        {
            SaveContent(key, stream, null);
        }

        public void SaveContent(string key, Stream stream, string message)
        {
            GitPull(true);

            var fullPath = Path.Combine(_basePath, key);
            using (var writeStream = OpenFile(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.CopyTo(writeStream);
            }

            using (var repo = GetRepository())
            {
                GitStage(repo, key);
                GitCommit(repo, message);
                GitPush(repo);
            }
        }

        public Stream ReadContent(string key)
        {
            GitPull();

            var fullPath = GetFullPath(key);
            return OpenFile(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void GitStage(params string[] paths)
        {
            using (var repo = new Repository(_basePath))
            {
                GitStage(repo, paths);
            }
        }

        private void GitStage(Repository repository, params string[] paths)
        {
            if (paths.Length == 1)
            {
                Commands.Stage(repository, paths.Single());
            }
            else
            {
                Commands.Stage(repository, paths);
            }
        }

        private void GitCommit(string message)
        {
            using (var repo = GetRepository())
            {
                GitCommit(repo, message);
            }
        }

        private void GitCommit(Repository repository, string message)
        {
            var signature = new Signature(_authorName, _authorEmail, DateTimeOffset.Now);
            repository.Commit(message, signature, signature);
        }

        private void GitPush()
        {
            using (var repo = GetRepository())
            {
                GitPush(repo);
            }
        }

        private Repository GetRepository()
        {
            return new Repository(_basePath);
        }

        private void GitPush(Repository repository)
        {
            var pushOptions = new PushOptions()
            {
                CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials
                    {
                        Username = _username,
                        Password = _password
                    })
            };

            var remote = repository.Network.Remotes[RemoteName];
            var branch = repository.Branches[$"{remote.Name}/{_branchName}"];
            repository.Network.Push(branch, pushOptions);
        }

        private void GitPull(bool force = false)
        {
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
                Repository.Init(_basePath);
            }

            using (var repo = GetRepository())
            {
                GitPull(repo, force);
            }
        }

        private void GitPull(Repository repository, bool force)
        {
            var coreDepth = repository.Config.GetValueOrDefault<string>("core.depth");
            if (coreDepth != "1")
            {
                repository.Config.Set("core.depth", "1");
            }

            var remote = repository.Network.Remotes[RemoteName]
                ?? repository.Network.Remotes.Add(RemoteName, _url);

            var fetchHeadFile = new FileInfo(Path.Combine(_basePath, ".git", "FETCH_HEAD"));
            if (force || !fetchHeadFile.Exists || fetchHeadFile.LastWriteTimeUtc < DateTime.UtcNow.Subtract(_cacheTtl))
            {
                var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(repository, remote.Name, refSpecs, null, null);

                var branch = repository.Branches[$"{remote.Name}/{_branchName}"];
                Commands.Checkout(repository, branch);
            }
        }

        private string GetFullPath(string path)
        {
            return Path.Combine(_basePath, path);
        }

        private FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
        {
            int maxTries = 10;
            int tries = 0;

            FileStream stream;
            while (!TryOpenFile(path, mode, access, share, out stream))
            {
                tries++;
                int maxSleepMs = 100 * (int)Math.Pow(2, tries);
                int sleepMs = _random.Next(maxSleepMs);

                Thread.Sleep(sleepMs);

                if (tries >= maxTries)
                {
                    throw new Exception("Can't open file: " + path);
                }
            }

            return stream;
        }

        private bool TryOpenFile(string path, FileMode mode, FileAccess access, FileShare share, out FileStream stream)
        {
            try
            {
                stream = File.Open(path, mode, access, share);
                return true;
            }
            catch
            {
                stream = null;
                return false;
            }
        }
    }
}
