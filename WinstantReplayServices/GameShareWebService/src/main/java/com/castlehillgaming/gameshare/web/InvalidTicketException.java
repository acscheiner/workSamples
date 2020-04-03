/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.web;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.ResponseStatus;

/**
 * The Class InvalidTicketException is thrown when a ticket is referenced using
 * an incorrect form of ticket identifier (i.e., is not a match for a UUID
 * string).
 */
@ResponseStatus(HttpStatus.BAD_REQUEST)
public class InvalidTicketException extends RuntimeException {

    /** The serialVersionUID. */
    private static final long serialVersionUID = -4856328694059068891L;

    /**
     * Instantiates a new invalid ticket exception.
     */
    public InvalidTicketException() {
        super("invalid ticket.");
    }

}
