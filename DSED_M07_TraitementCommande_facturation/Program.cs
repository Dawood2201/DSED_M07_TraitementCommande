using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using DSED_M07_Entites;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        string requetesSujet = "commande.placee";

        ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
        using (IConnection connection = factory.CreateConnection())
        {
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "m07-commandes",
                                        type: ExchangeType.Topic,
                                        durable: true,
                                        autoDelete: false);

                channel.QueueDeclare(queue: "m07-facturation",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueBind(queue: "m07-facturation",
                                  exchange: "m07-commandes",
                                  routingKey: requetesSujet);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    const double TAXES = 14.975;

                    byte[] body = ea.Body.ToArray();
                    string json = Encoding.UTF8.GetString(body);
                    Enveloppe enveloppe = JsonConvert.DeserializeObject<Enveloppe>(json);

                    switch (enveloppe.Type)
                    {
                        case TypeEnveloppe.normal:

                            break;

                        case TypeEnveloppe.premium:

                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Erreur lors du traitement d'une enveloppe, celle-ci n'a pas été traité.");
                            Console.ForegroundColor= ConsoleColor.White;
                            break;
                    }
                };
            }
        }
    }
}