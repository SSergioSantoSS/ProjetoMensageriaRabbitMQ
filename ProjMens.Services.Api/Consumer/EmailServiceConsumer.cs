using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProjMens.Services.Api.Contexts.Entities;
using ProjMens.Services.Api.Helpers;
using ProjMens.Services.Api.Models;
using ProjMens.Services.Api.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ProjMens.Services.Api.Consumer
{
    public class EmailServiceConsumer : BackgroundService
    {
        //atributos
        private readonly RabbitMQSettings? _rabbitMQSettings;
        private readonly IConnection? _connection;
        private readonly IModel? _model;
        private readonly IServiceProvider? _serviceProvider;
        private readonly EmailHelper _emailHelper;
        private readonly LogHelper _logHelper;

        //Construtor para injeção de depedência
        public EmailServiceConsumer(IOptions<RabbitMQSettings> options, IServiceProvider serviceProvider, EmailHelper emailHelper, LogHelper logHelper)
        {
            _rabbitMQSettings = options.Value;
            _serviceProvider = serviceProvider;
            _emailHelper = emailHelper;
            _logHelper = logHelper;

            //conectando no servidor de mensageria
            var connectionFactory = new ConnectionFactory
            {
                HostName = _rabbitMQSettings.Host,
                UserName = _rabbitMQSettings.Username,
                Password = _rabbitMQSettings.Password,
            };

            _connection = connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(
                    queue: _rabbitMQSettings.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
        }

        //método utilizado para ler as mensagens que estão na fila
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //criando o componente para executar as chamadas na fila (consumer)
            var consumer = new EventingBasicConsumer(_model);

            consumer.Received += (sender, args) =>
            {
                //lendo o conteudo da mensagem contida na fila
                var contentArray = args.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);

                //deserializando a mensagem lida na fila
                var message = JsonConvert.DeserializeObject<MessageViewModel>(contentString);

                //processando a mensagem
                using (var scope = _serviceProvider.CreateScope())
                {
                    //verificando se a mensagem é para este consumidor
                    if ("PostUsuarios".Equals(message.From) && "EmailService".Equals(message.To))
                    {
                        //deserializando o conteudo da mensagem
                        var usuario = JsonConvert.DeserializeObject<Usuario?>(message.Content);

                        //enviando o email
                        SendMail(usuario);

                        //comunicando que a mensagem da fila foi processada!
                        _model.BasicAck(args.DeliveryTag, false);
                    }
                }
            };

            _model.BasicConsume(_rabbitMQSettings.Queue, false, consumer);
            return Task.CompletedTask;
        }

        //método privado para montar e enviar o email
        private void SendMail(Usuario usuario)
        {
            var emailTo = usuario.Email;
            var subject = $"Confirmação de cadastro de usuário. ID: { usuario.Id} ";
            var body = $@"
                Olá {usuario.Nome}, 
                <br/>
                <br/>
                <strong>Parabéns, sua conta de usuário foi cadastrada com sucesso!</strong>
                <br/>
                <br/>
                ID do Usuário: <strong>{usuario.Id}</strong> <br/>
                Nome: <strong>{usuario.Nome}</strong> <br/>
                CPF: <strong>{usuario.Cpf}</strong> <br/>
                Email: <strong>{usuario.Email}</strong> <br/>
                <br/>
                Att,
                <br/>
                Equipe Mens.
            ";

            try
            {
                _emailHelper.Send(emailTo, subject, body);

                _logHelper
                    .Create($"Email enviado com sucesso: { JsonConvert.SerializeObject(usuario)}.", 
                            LogType.INFO);
            }
            catch (Exception e)
            {
                _logHelper
                    .Create($"Erro ao enviar email: {e.Message} – { JsonConvert.SerializeObject(usuario)}.", 
                            LogType.ERROR);
            }


            
        }
    }
}

            


