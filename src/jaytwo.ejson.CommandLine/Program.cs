using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace jaytwo.ejson.CommandLine
{
    public class Program
    {
        public static int Main(string[] args) => new Program().Run(args);

        private readonly IEJsonCrypto _eJsonCrypto;
        private readonly TextWriter _standardOut;
        private readonly TextWriter _standardError;


        public Program()
            : this(new EJsonCrypto(), Console.Out, Console.Error)
        {
        }

        internal Program(IEJsonCrypto eJsonCrypto, TextWriter standardOut, TextWriter standardError)
        {
            _eJsonCrypto = eJsonCrypto;
            _standardOut = standardOut;
            _standardError = standardError;
        }

        public int Run(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "ejson";
            app.HelpOption("--help");
            app.VersionOption("--version", "v0.1");
            app.Out = _standardOut;
            app.Error = _standardError;

            SetupKeygenCommand(app);
            SetupEncryptCommand(app);
            SetupDecryptCommand(app);

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }

        private void SetupKeygenCommand(CommandLineApplication app)
        {
            app.Command("keygen", context =>
            {
                context.Description = "generates a new key";
                context.HelpOption("--help");

                var writeOption = context.Option("-w", "writes to disk", CommandOptionType.NoValue);

                context.OnExecute(() =>
                {
                    var keyPair = _eJsonCrypto.GenerateKeyPair(writeOption.HasValue());
                    context.Out.WriteLine(keyPair);

                    return 0;
                });
            });
        }

        private void SetupEncryptCommand(CommandLineApplication app)
        {
            app.Command("encrypt", context =>
            {
                context.Description = "encrypts the thing";
                context.HelpOption("--help");

                var fileNameArgument = context.Argument("<file name>", "name of the file to encrypt");

                var writeOption = context.Option("-w", "writes to disk", CommandOptionType.NoValue);

                context.OnExecute(() =>
                {
                    if (writeOption.HasValue())
                    {
                        _eJsonCrypto.Encrypt(fileNameArgument.Value);
                    }
                    else
                    {
                        var json = _eJsonCrypto.GetEncryptedJson(fileNameArgument.Value);
                        context.Out.WriteLine(json);
                    }

                    return 0;
                });
            });
        }

        private void SetupDecryptCommand(CommandLineApplication app)
        {
            app.Command("decrypt", context =>
            {
                context.Description = "decrypts the thing";
                context.HelpOption("--help");

                var fileNameArgument = context.Argument("<file name>", "name of the file to decrypt");

                var writeOption = context.Option("-w", "writes to disk", CommandOptionType.NoValue);

                context.OnExecute(() =>
                {
                    if (writeOption.HasValue())
                    {
                        _eJsonCrypto.Decrypt(fileNameArgument.Value);
                    }
                    else
                    {
                        var json = _eJsonCrypto.GetDecryptedJson(fileNameArgument.Value);
                        context.Out.WriteLine(json);
                    }

                    return 0;
                });
            });
        }
    }
}
