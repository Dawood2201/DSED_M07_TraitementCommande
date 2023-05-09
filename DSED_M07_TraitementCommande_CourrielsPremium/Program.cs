using RabbitMQ;
using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;
using DSED_M07_Entites;

public class Program
{
    public static void Main(string[] args)
    {
        string requetesSujet = "commande.placee.premium";

        ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
        using (IConnection connection = factory.CreateConnection())
        {
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "m07-commandes",
                                        type: ExchangeType.Topic,
                                        durable: true,
                                        autoDelete: false);

                channel.QueueDeclare(queue: "m07-courriel-premium",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueBind(queue: "m07-courriel-premium",
                                  exchange: "m07-commandes",
                                  routingKey: requetesSujet);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    string json = Encoding.UTF8.GetString(body);
                    Enveloppe enveloppe = JsonConvert.DeserializeObject<Enveloppe>(json);
                    Commande commande = enveloppe.Commande;

                    Console.WriteLine("Commande premium reçu. REF: " + commande.Reference);

                };

                channel.BasicConsume(queue: "m07-courriel-premium",
                        autoAck: true,
                        consumer: consumer);

                Console.In.ReadLine();
            }
        }
    }
}