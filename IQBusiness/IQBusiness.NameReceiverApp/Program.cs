using IQBusiness.RabbitMQ.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace IQBusiness.NameReceiverApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
	 .SetBasePath(Directory.GetCurrentDirectory())
	 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			IConfigurationRoot configuration = builder.Build();
			PrintReceivedNameMessage(configuration);
		}
		private static void PrintReceivedNameMessage(IConfigurationRoot configuration)
		{
			var hostName = configuration.GetSection("RabbitMQConnectionString")["HostName"];
			var userName = configuration.GetSection("RabbitMQConnectionString")["UserName"];
			var password = configuration.GetSection("RabbitMQConnectionString")["Password"];
			var messageQueueName = configuration.GetSection("QueueDetails")["Queue"];
			RabbitMQService rabbitMQService = new RabbitMQService(hostName, userName, password);
			rabbitMQService.MessageQueueName = messageQueueName;
			try
			{
				var connection = rabbitMQService.GetConnection();
				var channel = connection.CreateModel();
				channel.QueueDeclare(queue: rabbitMQService.MessageQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += Consumer_Received;
				channel.BasicConsume(queue: rabbitMQService.MessageQueueName, autoAck: false, consumer: consumer);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
		{
			var body = e.Body;
			string receivedName = "";
			if(IsMessageValid(body, ref receivedName))
			{
				Console.WriteLine($"Hello {receivedName}, I'm your father");
				Console.ReadLine();
			}
		}
		private static bool IsMessageValid(byte[] message, ref string receivedName)
		{
			bool isValid = true;
			try
			{
				string messageReceived = Encoding.UTF8.GetString(message);
				if(string.IsNullOrEmpty(messageReceived))
				{
					return false;
				}
				else
				{
					string[] strMessage = messageReceived.Split(',');
					if(strMessage.Length == 2)
					{
						receivedName = strMessage[1];
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception e)
			{
				isValid = false;
			}
			return isValid;
		}
	}
}
