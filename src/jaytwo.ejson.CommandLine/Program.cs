using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.CommandLineUtils;

namespace jaytwo.ejson.CommandLine
{
    public class Program
    {
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

        public static int Main(string[] args) => new Program().Run(args);

        public int Run(params string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "ejson";
            app.HelpOption("--help");
            app.VersionOption("--version", GetType().Assembly.GetName().Version.ToString());
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
                        var keyDir = keyDirOption.Value();
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
                        _eJsonCrypto.EncryptFile(fileNameArgument.Value);
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
                    var keyDir = keyDirOption.Value();

                    if (outputToFileOption.HasValue())
                    {
                        var output = _eJsonCrypto.SaveDecryptedJsonFromFile(fileNameArgument.Value, outputToFileOption.Value(), keyDir);
                        context.Out.WriteLine(output);
                    }
                    else
                    {
                        var json = _eJsonCrypto.GetDecryptedJsonFromFile(fileNameArgument.Value, keyDir);
                        context.Out.WriteLine(json);
                    }

                    return 0;
                });
            });
        }
    }
}
