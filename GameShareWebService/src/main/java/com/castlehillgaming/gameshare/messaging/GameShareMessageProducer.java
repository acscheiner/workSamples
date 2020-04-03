/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.messaging;

import com.castlehillgaming.gameshare.model.Ticket;

/**
 * The Interface GameShareMessageProducer provides methods to produce outgoing
 * JMS messages for processing game-share recall data.
 */
public interface GameShareMessageProducer {

    /**
     * Send game share process recall data message.
     *
     * @param gameShareTicket
     *            the game share ticket
     */
    void sendGameShareProcessRecallDataMessage(Ticket gameShareTicket);
}
