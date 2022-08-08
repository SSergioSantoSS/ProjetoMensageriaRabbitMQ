using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProjMens.Services.Api.Models;
using ProjMens.Services.Api.Settings;
using RabbitMQ.Client;
using System.Text;

namespace ProjMens.Services.Api.Produces
{
    public class ProduceMessage
    {
        //atributos
        private readonly ConnectionFactory? _connectionFactory;
        private readonly RabbitMQSettings? _rabbitMQSettings;

        //construtor
        public ProduceMessage(IOptions<RabbitMQSettings> options)
        {
            _rabbitMQSettings = options.Value;
            _connectionFactory = new ConnectionFactory
            {
                HostName = _rabbitMQSettings.Host,
                UserName = _rabbitMQSettings.Username,
                Password = _rabbitMQSettings.Password,
            };
        }

        //método para publicar uma mensagem na fila
        public void Publish(MessageViewModel model)
        {
            //conectando no servidor da mensageria
            using (var connection = _connectionFactory?.CreateConnection())
            {
                //criando um objeto na fila de mensagens
                using (var channel = connection?.CreateModel())
                {
                    //criando um objeto na fila de mensagens
                    channel?.QueueDeclare(
                            queue: _rabbitMQSettings?.Queue, //nome da fila
                            durable: true, //dados permanecerão na fila mesmo após reiniciar o RabbitMQ
                            exclusive: false, 
                            autoDelete: false,
                            arguments: null
                        );

                    //escrever um conteudo para gravar na fila
                    var json = JsonConvert.SerializeObject(model);
                    var bytes = Encoding.UTF8.GetBytes(json);

                    //gravando na fila!
                    channel?.BasicPublish(
                        exchange: string.Empty,
                        routingKey: _rabbitMQSettings?.Queue,
                        basicProperties: null,
                        body: bytes
                        );
                }
            }

        }
    }
}
