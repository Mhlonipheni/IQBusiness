using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
namespace IQBusiness.RabbitMQ.Shared
{
	public class RabbitMQService
	{
		private readonly string _hostName;
		private readonly string _userName;
		private readonly string _password;
		public RabbitMQService(string hostName, string userName, string password)
		{
			_hostName = hostName;
			_userName = userName;
			_password = password;
		}
		public string MessageQueueName { get; set; }
		public IConnection GetConnection()
		{
			ConnectionFactory connectionFactory = new ConnectionFactory();
			connectionFactory.HostName = _hostName;
			connectionFactory.UserName = _userName;
			connectionFactory.Password = _password;
			IConnection connection = null;
			try
			{
				connection = connectionFactory.CreateConnection();
			}

			catch (BrokerUnreachableException ex)
			{
				throw new BrokerUnreachableException(ex.InnerException);
			}
			catch (ConnectFailureException ex)
			{
				throw new ConnectFailureException(ex.Message,ex.InnerException);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return connection;
		}
	}
}
