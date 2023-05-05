using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using DSED_M07_Entites;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        string requetesSujet = "#";

        ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
        using (IConnection connection = factory.CreateConnection())
        {
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "m07-commandes",
                                        type: ExchangeType.Topic,
                                        durable: true,
                                        autoDelete: false);

                channel.QueueDeclare(queue: "m07-journal",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.QueueBind(queue: "m07-journal",
                                  exchange: "m07-commandes",
                                  routingKey: requetesSujet);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    string json = Encoding.UTF8.GetString(body);
                    Enveloppe enveloppe = JsonConvert.DeserializeObject<Enveloppe>(json);

                    if (!Directory.Exists("JournalCommandes"))
                    {
                        Directory.CreateDirectory("JournalCommandes");
                    }
                    else
                    {
                        DateTime dateActuelle = DateTime.Now;

                        string path = $"JournalCommandes/{dateActuelle.Year}{dateActuelle.Month}{dateActuelle.Day}_{dateActuelle.Hour}{dateActuelle.Minute}{dateActuelle.Second}_{Guid.NewGuid()}.json";
                        string contenu = JsonConvert.SerializeObject(enveloppe, Formatting.Indented);

                        File.WriteAllText(path, contenu);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Fichier Créé ! ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"Path: {path}");
                    }
                };

                channel.BasicConsume(queue: "m07-journal",
                                     autoAck: true,
                                     consumer: consumer);

                Console.In.ReadLine();
            }
        }
    }
}