/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.web;

import java.net.URI;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.castlehillgaming.gameshare.model.GameShareInfo;
import com.castlehillgaming.gameshare.model.Ticket;
import com.castlehillgaming.gameshare.service.GameShareService;

// TODO: Auto-generated Javadoc
/**
 * The ShareGameController class is a RESTful controller for the GameShare web
 * service. It provides a RESTful interface for HTTP requests into the GameShare
 * web service.
 */
@RestController
@RequestMapping(value = "/sharegame")
public class ShareGameController {

    /** The class logger. */
    @SuppressWarnings("unused")
    private static final Logger logger = LoggerFactory.getLogger(ShareGameController.class);

    /** The game share service. */
    @Autowired
    private GameShareService gameShareService;

    /**
     * Submit game play info to game sharing service for
     * (de-coupled/asynchronous) generation of game play share content.
     *
     * @param gameShareInfo
     *            the game share info (including game title, game-play results,
     *            etc.) in HTTP request body
     * @return A ticket (to be redeemed when game-play share content is ready
     *         for downloading and sharing).
     */
    @RequestMapping(method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE, produces = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<Ticket> submitGamePlayInfo(@RequestBody final GameShareInfo gameShareInfo) {
        return new ResponseEntity<>(gameShareService.submitGameShareJob(gameShareInfo), HttpStatus.CREATED);
    }

    /**
     * Reedeem ticket.
     *
     * @param ticketValue
     *            the ticket value
     * @return HTTP response entity containing only headers and status
     */
    @RequestMapping(value = "/redeemticket/{ticketValue}", method = RequestMethod.GET)
    public ResponseEntity<?> reedeemTicket(@PathVariable final String ticketValue) {
        Ticket.validateTicketValue(ticketValue);

        final HttpHeaders httpHeaders = new HttpHeaders();
        try {
            httpHeaders.setLocation(new URI(gameShareService.redeemTicket(ticketValue)));
        } catch (final Exception e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }

        return new ResponseEntity<>(null, httpHeaders, HttpStatus.OK);
    }

    /**
     * Redeem tickets for specified jobs. Action is to check is tickets for job
     * completion and return a list of completed jobs only. Jobs which are not
     * yet completed are ignored (and not included in the return Map).
     *
     * @param tickets
     *            a list of job tickets to redeem
     * @return the response entity containing a Map of completed jobs keyed by
     *         ticketUuid mapping to the URL of the completed video
     */
    @RequestMapping(value = "/redeemtickets/{tickets}", method = RequestMethod.GET)
    public ResponseEntity<Map<String, String>> redeemTickets(@PathVariable final List<String> tickets) {
        final List<String> validTickets = new ArrayList<>();
        tickets.forEach(ticket -> {
            try {
                Ticket.validateTicketValue(ticket);
                validTickets.add(ticket);
            } catch (final InvalidTicketException ex) {
                // no-op: we simply do not include
                // invalid tickets in list passed to
                // gameShareService
            }
        });

        return new ResponseEntity<>(gameShareService.redeemTickets(validTickets), HttpStatus.OK);
    }
}
