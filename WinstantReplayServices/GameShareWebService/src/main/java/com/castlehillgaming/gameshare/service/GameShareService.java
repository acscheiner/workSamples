/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.service;

import java.util.List;
import java.util.Map;

import com.castlehillgaming.gameshare.model.GameShareInfo;
import com.castlehillgaming.gameshare.model.Ticket;
import com.castlehillgaming.gameshare_commonutils.TicketStatusEnum;

/**
 * The Interface GameShareService provides public contract for making requests
 * on the Game Share Web Service.
 */
public interface GameShareService {

    /**
     * Submit a game share job.
     *
     * @param gameShareInfo
     *            the game share info
     * @return the ticket
     */
    Ticket submitGameShareJob(GameShareInfo gameShareInfo);

    /**
     * Redeem ticket.
     *
     * @param ticketValue
     *            the ticket value
     * @return URL (as string) of completed GameShare video recording
     */
    String redeemTicket(String ticketValue);

    /**
     * Redeem Job Tickets.
     *
     * @param tickets
     *            the tickets
     * @return Map of completed jobs keyed by ticketUuid mapped to URL for
     *         completed video of game-play
     */
    Map<String, String> redeemTickets(List<String> tickets);

    /**
     * Update job status.
     *
     * @param ticketValue
     *            the ticket value
     * @param jobStatus
     *            the job status
     */
    void updateJobStatus(String ticketValue, TicketStatusEnum jobStatus);

    /**
     * Save video.
     *
     * @param ticketValue
     *            the ticket value
     * @param videoBytes
     *            the recorded game-share video as a byte array
     */
    void saveVideo(String ticketValue, byte[] videoBytes);

    /**
     * Complete job.
     *
     * @param ticketValue
     *            the ticket value
     * @param videoUrl
     *            the video url
     */
    void completeJob(String ticketValue, String videoUrl);
}
