/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.messaging;

import java.io.Serializable;

import javax.jms.TextMessage;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.jms.core.JmsTemplate;
import org.springframework.stereotype.Component;

import com.castlehillgaming.gameshare.model.Ticket;
import com.castlehillgaming.gameshare_commonutils.SharedConstants;
import com.castlehillgaming.gameshare_commonutils.TicketStatusEnum;

/**
 * The Class GameShareMessageProducerImpl implements GameShareMessageProducer
 * methods.
 */
@Component
public class GameShareMessageProducerImpl implements GameShareMessageProducer, Serializable {

    /** The serialVersionUID. */
    private static final long serialVersionUID = 2395658024823769433L;

    /** The logger. */
    private static final Logger logger = LoggerFactory.getLogger(GameShareMessageProducerImpl.class);

    /** The jms template. */
    @Autowired
    private JmsTemplate jmsTemplate;

    /*
     * (non-Javadoc)
     *
     * @see com.castlehillgaming.gameshare.messaging.GameShareMessageProducer#
     * sendGameShareJobMessage(com.castlehillgaming.gameshare.model.Ticket)
     */
    @Override
    public void sendGameShareProcessRecallDataMessage(final Ticket gameShareTicket) {
        jmsTemplate.send(SharedConstants.RECALLDATA_PROCESSING_MESSAGE_QUEUE_NAME, session -> {
            final TextMessage message = session
                    .createTextMessage(gameShareTicket.getGameShareInfo().getGameRecallData());
            message.setStringProperty(SharedConstants.TICKET_MESSAGE_KEY, gameShareTicket.getTicketId());
            message.setStringProperty(SharedConstants.GAME_TITLE_MESSAGE_KEY,
                    gameShareTicket.getGameShareInfo().getGameTitle());
            message.setStringProperty(SharedConstants.CASINO_NAME_MESSAGE_KEY,
                    gameShareTicket.getGameShareInfo().getCasino());
            message.setStringProperty(SharedConstants.CABINET_TYPE_MESSAGE_KEY,
                    gameShareTicket.getGameShareInfo().getCabinetType());
            message.setLongProperty(SharedConstants.GAME_PLAY_TIME_MESSAGE_KEY,
                    gameShareTicket.getGameShareInfo().getGamePlayedAt());

            logger.debug("sending JMS message: " + message.getStringProperty(SharedConstants.TICKET_MESSAGE_KEY));

            return message;
        });

        gameShareTicket.setStatus(TicketStatusEnum.Dispatched);
    }
}
