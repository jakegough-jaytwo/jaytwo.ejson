using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.CommandLineUtils;

namespace jaytwo.ejson.CommandLine
{
    public class Program
    {
        public const string KeyDirEnvironmentVariable = "EJSON_KEYDIR";
        public const string DefaultUnixKeyDir = "/opt/ejson/keys";

        public static int Main(string[] args) => new Program().Run(args);

        private readonly IEJsonCrypto _eJsonCrypto;
        private readonly TextWriter _standardOut;
        private readonly TextWriter _standardError;


        public Program()
            : this(null, null, null)
        {
        }

        internal Program(IEJsonCrypto eJsonCrypto, TextWriter standardOut, TextWriter standardError)
        {
            _eJsonCrypto = eJsonCrypto ?? new EJsonCrypto();
            _standardOut = standardOut ?? Console.Out;
            _standardError = standardError ?? Console.Error;
        }

        public int Run(params string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "ejson";
            app.HelpOption("--help");
            app.VersionOption("--version", "v0.1");
            app.Out = _standardOut;
            app.Error = _standardError;


            var keyDirOption = app.Option("--keydir|-k", "Directory containing EJSON keys [$EJSON_KEYDIR]", CommandOptionType.SingleValue);

            SetupKeygenCommand(app, keyDirOption);
            SetupEncryptCommand(app);
            SetupDecryptCommand(app, keyDirOption);

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }

        private void SetupKeygenCommand(CommandLineApplication app, CommandOption keyDirOption)
        {
            //SetupKeygenCommand(app, keyDirOption, "g");
            SetupKeygenCommand(app, keyDirOption, "keygen");
        }

        private CommandLineApplication SetupKeygenCommand(CommandLineApplication app, CommandOption keyDirOption, string name)
        {
            return app.Command(name, context =>
            {
                context.Out = _standardOut;
                context.Error = _standardError;
                context.Description = "generate a new EJSON keypair";
                context.HelpOption("--help");

                var writeOption = context.Option("-w", "writes to disk", CommandOptionType.NoValue);

                context.OnExecute(() =>
                {
                    string output;
                    if (writeOption.HasValue())
                    {
                        var keyDir = GetKeyDirOrDefault(keyDirOption);
                        output = _eJsonCrypto.SaveKeyPair(keyDir);
                    }
                    else
                    {
                        output = _eJsonCrypto.GenerateKeyPair();
                    }

                    context.Out.WriteLine(output);
                    return 0;
                });
            });
        }

        private void SetupEncryptCommand(CommandLineApplication app)
        {
            //SetupEncryptCommand(app, "e");
            SetupEncryptCommand(app, "encrypt");
        }

        private CommandLineApplication SetupEncryptCommand(CommandLineApplication app, string name)
        {
            return app.Command(name, context =>
            {
                context.Out = _standardOut;
                context.Error = _standardError;
                context.Description = "(re-)encrypt one or more EJSON files";
                context.HelpOption("--help");

                var fileNameArgument = context.Argument("<files>", "names of the files to decrypt", true);

                context.OnExecute(() =>
                {
                    foreach (var value in fileNameArgument.Values)
                    {
                        _eJsonCrypto.Encrypt(fileNameArgument.Value);
                    }

                    return 0;
                });
            });
        }

        private void SetupDecryptCommand(CommandLineApplication app, CommandOption keyDirOption)
        {
            //SetupDecryptCommand(app, keyDirOption, "d");
            SetupDecryptCommand(app, keyDirOption, "decrypt");
        }

        private CommandLineApplication SetupDecryptCommand(CommandLineApplication app, CommandOption keyDirOption, string name)
        {
            return app.Command(name, context =>
            {
                context.Out = _standardOut;
                context.Error = _standardError;
                context.Description = "decrypt an EJSON file";
                context.HelpOption("--help");

                var fileNameArgument = context.Argument("<file>", "name of the file to decrypt");

                var outputToFileOption = context.Option("-o", "print output to the provided file, rather than stdout", CommandOptionType.SingleValue);

                context.OnExecute(() =>
                {
                    var keyDir = GetKeyDirOrDefault(keyDirOption);
                    
                    if (outputToFileOption.HasValue())
                    {
                        var output = _eJsonCrypto.SaveDecryptedJson(fileNameArgument.Value, outputToFileOption.Value(), keyDir);
                        context.Out.WriteLine(output);
                    }
                    else
                    {
                        var json = _eJsonCrypto.GetDecryptedJson(fileNameArgument.Value, keyDir);
                        context.Out.WriteLine(json);
                    }

                    return 0;
                });
            });
        }

        private string GetKeyDirOrDefault(CommandOption keyDirOption)
        {
            var keyDirOptionValue = keyDirOption.Value();
            return GetKeyDirOrDefault(keyDirOptionValue);
        }

        internal string GetKeyDirOrDefault(string keyDirOptionValue)
        {
            var keyDir = keyDirOptionValue;

            if (string.IsNullOrWhiteSpace(keyDir))
            {
                keyDir = Environment.GetEnvironmentVariable(KeyDirEnvironmentVariable);
            }

            if (string.IsNullOrWhiteSpace(keyDir))
            {
                keyDir = GetDefaultKeyDir();
            }

            return keyDir;
        }

        internal string GetDefaultKeyDir()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Environment.GetEnvironmentVariable("USERPROFILE") + "/.ejson/keys";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Environment.GetEnvironmentVariable("HOME") + "/.ejson/keys";
            }
            else
            {
                return DefaultUnixKeyDir;
            }
        }
    }
}
