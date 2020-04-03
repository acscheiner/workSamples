/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.messaging;

import javax.jms.BytesMessage;
import javax.jms.JMSException;
import javax.jms.Message;
import javax.jms.TextMessage;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.jms.annotation.JmsListener;
import org.springframework.stereotype.Component;

import com.castlehillgaming.gameshare.service.GameShareService;
import com.castlehillgaming.gameshare_commonutils.SharedConstants;
import com.castlehillgaming.gameshare_commonutils.TicketStatusEnum;

@Component
public class GameShareJobMessageListener {

    /** The logger. */
    private static Logger logger = LoggerFactory.getLogger(GameShareJobMessageListener.class);

    /** The game share service. */
    @Autowired
    private GameShareService gameShareService;

    /**
     * Consume message on the Game Share Job Message Queue.
     *
     * @param message
     *            the message
     */
    @JmsListener(destination = SharedConstants.JOB_INFO_MESSAGE_QUEUE_NAME)
    public void consumeMessage(final Message message) {
        try {
            final String ticketUuid = message.getStringProperty(SharedConstants.TICKET_MESSAGE_KEY);
            final TicketStatusEnum jobStatus = TicketStatusEnum
                    .getInstance(message.getIntProperty(SharedConstants.JOB_STATUS_MESSAGE_KEY));
            logger.debug("Received job status update message for job with Ticket UUID = " + ticketUuid
                    + "; new job status of " + jobStatus.toString());

            if (jobStatus.equals(TicketStatusEnum.Recorded)) {
                final BytesMessage bytesMessage = (BytesMessage) message;

                byte[] videoBytes = null;

                final long numBytes = bytesMessage.getBodyLength();
                logger.debug("length of BytesMessage byte[]: " + numBytes);

                if (numBytes <= Integer.MAX_VALUE) {
                    final int numBytesInt = (int) numBytes;
                    videoBytes = new byte[numBytesInt];
                    bytesMessage.readBytes(videoBytes);
                } else {
                    logger.warn("Size of video byte array in BytesMessage exceeds " + Integer.MAX_VALUE
                            + " - Cannot read message payload into byte[]");
                }

                gameShareService.saveVideo(ticketUuid, videoBytes);
            } else if (jobStatus.equals(TicketStatusEnum.Completed)) {
                final TextMessage textMessage = (TextMessage) message;
                final String videoUrl = textMessage.getText();
                gameShareService.completeJob(ticketUuid, videoUrl);
            } else {
                gameShareService.updateJobStatus(ticketUuid, jobStatus);
            }
        } catch (final JMSException e) {
            e.printStackTrace();
        }
    }

}
