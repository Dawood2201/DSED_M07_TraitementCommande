using RabbitMQ.Client;
using System.Text;
using DSED_M07_Entites;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

public class Program
{
    public static void Main(string[] args)
    {
        List<Commande>? commandes = GenererCommandes(10);

        if (commandes != null)
        {
            for (int index = 0; index < commandes.Count; index++)
            {
                TypeEnveloppe type = default;

                if (index % 2 == 0)
                {
                    type = TypeEnveloppe.normal;
                }
                else
                {
                    type = TypeEnveloppe.premium;
                }

                Enveloppe enveloppe = new Enveloppe(type, commandes[index]);
                EnvoyerEnveloppe(enveloppe);
            }

            Console.WriteLine("Appuyer sur [Entrer] pour fermer...");
            Console.In.ReadLine();
        }
    }

    private static void EnvoyerEnveloppe(Enveloppe p_enveloppe)
    {
        ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
        using(IConnection connection = factory.CreateConnection())
        {
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "m07-commandes", 
                                        type: ExchangeType.Topic,
                                        durable: true,
                                        autoDelete: false);

                string sujet = $"commande.placee.{p_enveloppe.Type.ToString()}";
                string json = JsonConvert.SerializeObject(p_enveloppe);
                                                                                                                                                
                byte[] body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(exchange: "m07-commandes", 
                                     routingKey: sujet, 
                                     basicProperties: null, 
                                     body: body);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Message Envoyé !  {sujet}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        } 
    }   

    private static List<Commande>? GenererCommandes(int p_nbDeCommandes)
    {
        try
        {
            if (p_nbDeCommandes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(p_nbDeCommandes), "Le nombre de commandes doit être supérieur à 0.");
            }

            List<Commande> commandes = new List<Commande>();

            for (int i = 0; i < p_nbDeCommandes; i++)
            {
                commandes.Add(Commande.GenererCommandeAleatoire());
            }

            return commandes;
        }
        catch (Exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Une erreur est survenue lors de la génération des commandes.");
            Console.ForegroundColor = ConsoleColor.White;

            return null;
        }
    }
}