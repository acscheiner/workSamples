/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.web;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

/**
 * The Class TicketNotFoundException is thrown when a ticket with a specified
 * UUID string is not found in the Game Share Service system.
 */
@ResponseStatus(HttpStatus.NOT_FOUND)
public class TicketNotFoundException extends RuntimeException {

    /** The serialVersionUID. */
    private static final long serialVersionUID = 4137122911727561012L;

    /**
     * Instantiates a new ticket not found exception.
     *
     * @param ticketValue
     *            the ticket value
     */
    public TicketNotFoundException(final String ticketValue) {
        super("could not find ticket '" + ticketValue + "'.");
    }

    /**
     * Instantiates a new ticket not found exception.
     */
    public TicketNotFoundException() {
        super("could not find ticket.");
    }
}
