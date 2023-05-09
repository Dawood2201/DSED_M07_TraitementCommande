using DSED_M07_Entites;
using RabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        string requetesSujet = "commande.placee.*";

        ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
        using (IConnection connection = factory.CreateConnection())
        {
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "m07-commandes",
                                        type: ExchangeType.Topic,
                                        durable: true,
                                        autoDelete: false);

                channel.QueueDeclare(queue: "m07-preparation-expedition",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueBind(queue: "m07-preparation-expedition",
                                  exchange: "m07-commandes",
                                  routingKey: requetesSujet);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    string json = Encoding.UTF8.GetString(body);
                    Enveloppe enveloppe = JsonConvert.DeserializeObject<Enveloppe>(json);
                    Commande commande = enveloppe.Commande;

                    switch (enveloppe.Type)
                    {
                        case TypeEnveloppe.normal:
                            Console.WriteLine("Préparez les articles suivants dans un emballage NORMAL:");
                            foreach (Article article in commande.Articles)
                            {
                                Console.WriteLine(article.Nom + " Qty: " + article.Quantitee);
                            }
                            break;

                        case TypeEnveloppe.premium:
                            Console.WriteLine("Préparez les articles suivants dans un emballage PREMIUM:");
                            foreach (Article article in commande.Articles)
                            {
                                Console.WriteLine(article.Nom + " Qty: " + article.Quantitee);
                            }
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Erreur lors du traitement d'une enveloppe, celle-ci n'a pas été traité.");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }
                };

                channel.BasicConsume(queue: "m07-preparation-expedition",
                        autoAck: true,
                        consumer: consumer);

                Console.In.ReadLine();
            }
        }
    }
}
