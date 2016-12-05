namespace CITYMumbler.Common.Data
{
	public class Client
	{
		public ushort ID { get; private set; }
		public string Name { get; private set; }

		public Client(ushort id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}
}
