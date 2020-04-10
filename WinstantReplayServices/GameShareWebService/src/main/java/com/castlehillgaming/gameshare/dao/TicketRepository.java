/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.dao;

import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;

import com.castlehillgaming.gameshare.model.Ticket;

/**
 * The Spring JPA Repository for data store entities of type Ticket.
 */
public interface TicketRepository extends JpaRepository<Ticket, Long> {

    /**
     * Find the Ticket entity with the specified ticketId.
     *
     * @param ticketId the ticketId value used for uniquely ID-ing a Winstant Reply
     *                 request
     * @return an Optional Ticket instance (which will be null-valued if no entity
     *         was found with the specified ticketId String).
     */
    Optional<Ticket> findByTicketId(String ticketId);
}
