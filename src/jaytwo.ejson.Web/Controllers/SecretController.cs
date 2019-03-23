using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using LibGit2Sharp.Handlers;
using jaytwo.ejson.Web.Data;

namespace jaytwo.ejson.Web.Controllers
{
    public class SecretController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var workingDirectory = Path.Combine(Path.GetTempPath(), "gitsecrets");
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }

            var url = "https://github.com/jakegough/jaytwo.ejson.git";
            var branch = "develop";

            var hash = Hash(url, branch);
            var gitPath = Path.Combine(workingDirectory, hash);

            var datastore = new GitDatastore(url, branch, gitPath, "nobody", "nobody@example.com", "username", "password");
            var allFiles = datastore.GetFiles("*");
            var response = string.Join(",", allFiles);

            //DeleteWithBackoff(tempFolder);

            return Content(response);
        }

        private string Hash(params string[] values)
        {
            var valuesAsString = string.Join("\n", values);

            using (var sha = SHA256.Create())
            {
                var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(valuesAsString));
                var hashString = Convert.ToBase64String(hashBytes).Replace("/", "-").Replace("=", "");
                return hashString;
            }
        }

        private void DeleteWithBackoff(string path)
        {
            var count = 0;
            while (!TryDelete(path))
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));

                if (count++ > 10)
                {
                    throw new Exception("Cannot delete!");
                }
            }
        }

        private bool TryDelete(string path)
        {
            try
            {
                Directory.Delete(path, true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
