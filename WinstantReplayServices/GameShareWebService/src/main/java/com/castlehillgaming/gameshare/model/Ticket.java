/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.model;

import java.io.Serializable;
import java.text.DateFormat;
import java.util.Calendar;
import java.util.UUID;
import java.util.regex.Pattern;

import javax.persistence.Column;
import javax.persistence.Embedded;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Version;

import org.hibernate.annotations.Type;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.castlehillgaming.gameshare.web.InvalidTicketException;
import com.castlehillgaming.gameshare_commonutils.SharedConstants;
import com.castlehillgaming.gameshare_commonutils.TicketStatusEnum;
import com.fasterxml.jackson.annotation.JsonIgnore;

import lombok.AccessLevel;
import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * The Ticket class encapsulates a ticket issued by the GameShare web service to
 * clients making requests to share their game-play. The ticket is redeemed when
 * the game-play share content has been created and is available for downloading
 * and sharing.
 */
@Entity
@EqualsAndHashCode(of = { "ticketId" })
@NoArgsConstructor(access = AccessLevel.PRIVATE, force = true)
public class Ticket implements Serializable {

    /** The serialVersionUID. */
    private static final long serialVersionUID = -7579181190621555097L;

    /** The class logger. */
    @SuppressWarnings("unused")
    private static final Logger logger = LoggerFactory.getLogger(Ticket.class);

    public static final String ENCRYPTED_STRING_TYPENAME = "encryptedFixedSaltString";

    /** The id - primary key for the associated db entity. */
    @Id
    @GeneratedValue(strategy = GenerationType.AUTO)
    @JsonIgnore
    private @Getter Long id;

    /**
     * Provides support for optimistic locking using a version property.
     */
    @Version
    @JsonIgnore
    private int version;

    /**
     * The unique ticket ID used for uniquely identifying the Winstant Replay
     * request and all of its associated data.
     */
    @Column(unique = true, nullable = false)
    @Type(type = ENCRYPTED_STRING_TYPENAME)
    private @Getter String ticketId;

    /** The game share info associated with this ticket. */
    @Embedded
    @JsonIgnore
    @Column(nullable = false)
    private @Getter GameShareInfo gameShareInfo;

    /** The status. */
    @Embedded
    @Column(nullable = false)
    private @Getter TicketStatus status;

    /**
     * The URL pointing to the location of the uploaded video for this game-share.
     */
    @JsonIgnore
    private @Getter @Setter String videoUrl;

    /** The recorded game-share video as a byte array. */
    @JsonIgnore
    private @Getter @Setter byte[] videoBytes;

    /**
     * Static class method for creating a new ticket.
     *
     * @param gameShareInfo the game share info
     * @return the newly created ticket
     */
    static public Ticket createTicket(final GameShareInfo gameShareInfo) {
        return new Ticket(gameShareInfo);
    }

    /**
     * Instantiates a new ticket.
     *
     * @param gameShareInfo the game share info
     */
    private Ticket(final GameShareInfo gameShareInfo) {
        this.gameShareInfo = gameShareInfo;
        ticketId = UUID.randomUUID().toString();
        status = TicketStatus.createNewTicketStatus();
    }

    /**
     * Validate ticketId value.
     *
     * @param ticketValue the ticketId value
     */
    public static void validateTicketValue(final String ticketValue) {
        if (!Pattern.compile("^" + SharedConstants.ticketIdRegex + "$").matcher(ticketValue).matches())
            throw new InvalidTicketException();
    }

    /**
     * Claim this ticket.
     *
     * @return true, if this call resulted in ticket claim; false otherwise.
     */
    public boolean claim() {
        return status.claim();
    }

    /**
     * Sets the status.
     *
     * @param status the new status
     */
    public void setStatus(final TicketStatusEnum status) {
        this.status.setStatus(status);
    }

    /*
     * (non-Javadoc)
     *
     * @see java.lang.Object#toString()
     */
    @Override
    public String toString() {
        final Calendar utcCalendar = new Calendar.Builder().setCalendarType("gregory").setTimeZone(SharedConstants.utc)
                .build();
        final DateFormat utcDateFormat = DateFormat.getDateTimeInstance();
        utcDateFormat.setCalendar(utcCalendar);
        return "Ticket [ticketId=" + ticketId + " " + status.toString() + " " + gameShareInfo.toString() + "]";
    }
}
