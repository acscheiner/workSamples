/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.model;

import java.io.Serializable;
import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Embeddable;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;

import com.castlehillgaming.gameshare_commonutils.SharedConstants;
import com.castlehillgaming.gameshare_commonutils.TicketStatusEnum;

import lombok.AccessLevel;
import lombok.AllArgsConstructor;
import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

/**
 * The Class TicketStatus encapsulates the status and date-time milestones for
 * processing of a Game-Share Ticket.
 */
@Embeddable
@EqualsAndHashCode
@ToString
@NoArgsConstructor(access = AccessLevel.PRIVATE)
@AllArgsConstructor(access = AccessLevel.PRIVATE)
class TicketStatus implements Serializable {

    /** The Constant serialVersionUID. */
    private static final long serialVersionUID = -7080041690341341329L;

    /**
     * The date-time that the ticket was created at.
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date createdAt;

    /**
     * The date-time that the ticket was dispatched for processing (set to
     * dateZero if not yet dispatched).
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date dispatchedAt;

    /**
     * The date-time that video recording began for this ticket (set to dateZero
     * if recording has not yet started).
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date recordingAt;

    /**
     * The date-time that video recording completed for this ticket (set to
     * dateZero if recording has not yet completed).
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date recordedAt;

    /**
     * The date-time that video uploading began for this ticket (set to dateZero
     * if recording has not yet started).
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date uploadingAt;

    /**
     * The date-time that video uploading completed for this ticket (set to
     * dateZero if recording has not yet completed).
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date uploadedAt;

    /**
     * The date-time that all processing completed for this ticket (set to
     * dateZero if processing has not yet fully completed).
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date completedAt;

    /**
     * The date-time that processing failed for this ticket (set to dateZero if
     * processing has not yet - or did not ever - failed).
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date failedAt;

    /**
     * The date-time that the ticket was claimed at (set to dateZero if not yet
     * claimed).
     */
    @Column(columnDefinition = "timestamp (3) with time zone", nullable = false)
    private @Getter Date claimedAt;

    /** The ticket status. */
    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private @Getter TicketStatusEnum status;

    /**
     * Creates a ticket status instance for a new ticket.
     *
     * @return the ticket status
     */
    static TicketStatus createNewTicketStatus() {
        return new TicketStatus(new Date(), SharedConstants.dateZero, SharedConstants.dateZero,
                SharedConstants.dateZero, SharedConstants.dateZero, SharedConstants.dateZero, SharedConstants.dateZero,
                SharedConstants.dateZero, SharedConstants.dateZero, TicketStatusEnum.Received);
    }

    /**
     * Sets the ticket status.
     *
     * @param status
     *            the new status
     */
    void setStatus(final TicketStatusEnum status) {
        // only permit status transitions in ordered way
        // (though a status of Failed can occur at any point)
        if (status.getIntValue() > this.status.getIntValue() || status.equals(TicketStatusEnum.Failed)) {
            this.status = status;
            final Date statusChangedAt = new Date();

            switch (this.status) {
            case Dispatched:
                dispatchedAt = statusChangedAt;
                break;

            case Recording:
                recordingAt = statusChangedAt;
                break;

            case Recorded:
                recordedAt = statusChangedAt;
                break;

            case Uploading:
                uploadingAt = statusChangedAt;
                break;

            case Uploaded:
                uploadedAt = statusChangedAt;
                break;

            case Completed:
                completedAt = statusChangedAt;
                break;

            case Failed:
                failedAt = statusChangedAt;
                break;

            case Claimed:
                claimedAt = statusChangedAt;
                break;

            default:
                break;
            }
        }
    }

    /**
     * Claim the ticket.
     *
     * @return true, if ticket could be claimed; false otherwise
     */
    boolean claim() {
        boolean didClaim = false;

        if (status.equals(TicketStatusEnum.Completed)) {
            setStatus(TicketStatusEnum.Claimed);
            didClaim = true;
        }

        return didClaim;
    }
}
