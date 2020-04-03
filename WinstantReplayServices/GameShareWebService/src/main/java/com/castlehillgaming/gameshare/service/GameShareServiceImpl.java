/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.service;

import java.io.Serializable;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.castlehillgaming.gameshare.dao.TicketRepository;
import com.castlehillgaming.gameshare.messaging.GameShareMessageProducer;
import com.castlehillgaming.gameshare.model.GameShareInfo;
import com.castlehillgaming.gameshare.model.Ticket;
import com.castlehillgaming.gameshare.web.TicketNotFoundException;
import com.castlehillgaming.gameshare_commonutils.TicketStatusEnum;

/**
 * The Class GameShareServiceImpl.
 */
@Service
@Transactional(readOnly = true)
public class GameShareServiceImpl implements GameShareService, Serializable {

    /** The serialVersionUID. */
    private static final long serialVersionUID = 8765123697852392633L;

    /** The ticket repository. */
    @Autowired
    private TicketRepository ticketRepo;

    /** The message producer. */
    @Autowired
    private GameShareMessageProducer msgProducer;

    /*
     * (non-Javadoc)
     *
     * @see com.castlehillgaming.gameshare.service.GameShareService#
     * submitGameShareJob (com.castlehillgaming.gameshare.model.GameShareInfo)
     */
    @Override
    @Transactional
    public Ticket submitGameShareJob(final GameShareInfo gameShareInfo) {
        final Ticket ticket = Ticket.createTicket(gameShareInfo);
        msgProducer.sendGameShareProcessRecallDataMessage(ticket);
        ticketRepo.save(ticket);
        return ticket;
    }

    /*
     * (non-Javadoc)
     *
     * @see
     * com.castlehillgaming.gameshare.service.GameShareService#redeemTicket(
     * java.lang.String)
     */
    @Override
    @Transactional
    public String redeemTicket(final String ticketValue) {
        final Ticket redeemedTicket = ticketRepo.findByTicketId(ticketValue).map(ticket -> {
            if (ticket.claim()) {
                ticketRepo.save(ticket);
            }

            return ticket;
        }).orElseThrow(TicketNotFoundException::new);

        return redeemedTicket.getVideoUrl();
    }

    /*
     * (non-Javadoc)
     *
     * @see
     * com.castlehillgaming.gameshare.service.GameShareService#updateJobStatus(
     * java.lang.String,
     * com.castlehillgaming.gameshare_commonutils.TicketStatusEnum)
     */
    @Override
    @Transactional
    public void updateJobStatus(final String ticketValue, final TicketStatusEnum jobStatus) {
        ticketRepo.findByTicketId(ticketValue).map(ticket -> {
            ticket.setStatus(jobStatus);
            ticketRepo.save(ticket);
            return ticket;
        }).orElseThrow(TicketNotFoundException::new);
    }

    /*
     * (non-Javadoc)
     *
     * @see
     * com.castlehillgaming.gameshare.service.GameShareService#completeJob(java.
     * lang.String, java.lang.String)
     */
    @Override
    @Transactional
    public void completeJob(final String ticketValue, final String videoUrl) {
        ticketRepo.findByTicketId(ticketValue).map(ticket -> {
            ticket.setVideoUrl(videoUrl);
            ticket.setStatus(TicketStatusEnum.Completed);
            ticketRepo.save(ticket);
            return ticket;
        }).orElseThrow(TicketNotFoundException::new);
    }

    /*
     * (non-Javadoc)
     *
     * @see
     * com.castlehillgaming.gameshare.service.GameShareService#redeemTickets(
     * java.util.Map)
     */
    @Override
    @Transactional
    public Map<String, String> redeemTickets(final List<String> tickets) {
        final Map<String, String> completedJobs = new HashMap<>();

        tickets.forEach(ticketUuid -> {
            ticketRepo.findByTicketId(ticketUuid).map(ticket -> {
                final String videoUrl = ticket.getVideoUrl();
                if (null != videoUrl) {
                    completedJobs.put(ticketUuid, videoUrl);
                    if (ticket.claim()) {
                        ticketRepo.save(ticket);
                    }
                }
                return ticket;
            });
        });

        return completedJobs;
    }

    @Override
    @Transactional
    public void saveVideo(final String ticketValue, final byte[] videoBytes) {
        ticketRepo.findByTicketId(ticketValue).map(ticket -> {
            ticket.setVideoBytes(videoBytes);
            ticket.setStatus(TicketStatusEnum.Recorded);
            ticketRepo.save(ticket);
            return ticket;
        }).orElseThrow(TicketNotFoundException::new);
    }
}
