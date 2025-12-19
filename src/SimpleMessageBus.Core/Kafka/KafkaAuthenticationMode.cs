namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// SASL authentication mechanisms for Kafka.
    /// </summary>
    public enum KafkaAuthenticationMode
    {

        /// <summary>No authentication configured.</summary>
        NotSet = 0,

        /// <summary>PLAIN mechanism (username/password).</summary>
        Plain = 1,

        /// <summary>SCRAM-SHA-256 mechanism.</summary>
        ScramSha256 = 2,

        /// <summary>SCRAM-SHA-512 mechanism.</summary>
        ScramSha512 = 3,

        /// <summary>Kerberos (GSSAPI) authentication.</summary>
        Gssapi = 4

    }

}
