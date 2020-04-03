/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.model;


import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import com.castlehillgaming.gameshare.web.InvalidTicketException;
import com.castlehillgaming.gameshare_commonutils.SharedConstants;
import com.castlehillgaming.gameshare_commonutils.TicketStatusEnum;

public class TicketTest {

    private GameShareInfo gameShareInfo;
    private Ticket ticket;

    @BeforeEach
    public void setUp() throws Exception {
        gameShareInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "ArcticCash",
                Integer.valueOf(1200000), "wow! what a win!");
        ticket = Ticket.createTicket(gameShareInfo);
    }

    @Test
    public void testCreateTicket() {
        final Ticket newTicket = Ticket.createTicket(gameShareInfo);
        assertNotNull(newTicket);
        assertTrue(SharedConstants.dateZero.getTime() < newTicket.getStatus().getCreatedAt().getTime());
        assertEquals(SharedConstants.dateZero.getTime(), newTicket.getStatus().getDispatchedAt().getTime());
        assertEquals(SharedConstants.dateZero.getTime(), newTicket.getStatus().getRecordingAt().getTime());
        assertEquals(SharedConstants.dateZero.getTime(), newTicket.getStatus().getRecordedAt().getTime());
        assertEquals(SharedConstants.dateZero.getTime(), newTicket.getStatus().getUploadingAt().getTime());
        assertEquals(SharedConstants.dateZero.getTime(), newTicket.getStatus().getUploadedAt().getTime());
        assertEquals(SharedConstants.dateZero.getTime(), newTicket.getStatus().getCompletedAt().getTime());
        assertEquals(SharedConstants.dateZero.getTime(), newTicket.getStatus().getClaimedAt().getTime());
        assertEquals(SharedConstants.dateZero.getTime(), newTicket.getStatus().getFailedAt().getTime());
        assertEquals(TicketStatusEnum.Received, newTicket.getStatus().getStatus());
    }

    @Test
    public void testClaimTicket() {
        assertEquals(SharedConstants.dateZero.getTime(), ticket.getStatus().getClaimedAt().getTime());
        assertFalse(ticket.claim());
    }

    @Test
    public void testValidateValidTicket() {
        Ticket.validateTicketValue(ticket.getTicketId());
        assertTrue(true);
    }

    @Test
    public void testValidateInvalidTicket() {
        assertThrows(InvalidTicketException.class, () -> {
        	Ticket.validateTicketValue("not a ticket value");
          });
    }

    @Test
    public void testObjEquality() {
        final Ticket newTicket = Ticket.createTicket(gameShareInfo);
        assertFalse(ticket.equals(newTicket));
    }
}
