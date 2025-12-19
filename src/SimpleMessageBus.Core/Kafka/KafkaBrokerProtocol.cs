namespace CloudNimble.SimpleMessageBus.Core
{

    /// <summary>
    /// Kafka security protocol options.
    /// </summary>
    public enum KafkaBrokerProtocol
    {

        /// <summary>Plain text communication (no encryption).</summary>
        Plaintext = 0,

        /// <summary>SSL/TLS encrypted communication.</summary>
        Ssl = 1,

        /// <summary>SASL authentication over plain text.</summary>
        SaslPlaintext = 2,

        /// <summary>SASL authentication over SSL/TLS.</summary>
        SaslSsl = 3

    }

}
