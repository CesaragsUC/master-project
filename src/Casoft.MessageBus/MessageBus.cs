﻿using MassTransit;

namespace Casoft.MessageBus;

public class MessageBus : IMessageBus
{
    private readonly IPublishEndpoint _publish;
    public MessageBus(IPublishEndpoint publish)
    {
        _publish = publish;
    }


    public async Task PublishAsync<T>(T message)
    {
        if (message != null)
        {
            await _publish.Publish(message);
        }
        else
        {
            throw new ArgumentNullException(nameof(message));
        }
    }
}