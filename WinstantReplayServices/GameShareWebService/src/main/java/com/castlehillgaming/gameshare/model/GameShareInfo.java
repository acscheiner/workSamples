/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.model;

import java.io.Serializable;

import javax.persistence.Column;
import javax.persistence.Embeddable;
import javax.validation.constraints.NotNull;

import org.apache.commons.lang3.StringUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import lombok.AccessLevel;
import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

/**
 * The Class GameShareInfo encapsulates the data that player posts to the Game
 * Share Service when s/he initiates a game-share request.
 */
@Embeddable
@ToString
@EqualsAndHashCode
@NoArgsConstructor(access = AccessLevel.PRIVATE, force = true)
public class GameShareInfo implements Serializable {

    /** The serialVersionUID. */
    private static final long serialVersionUID = -5207789309148107210L;

    /** The Constant logger. */
    @SuppressWarnings("unused")
    private static final Logger logger = LoggerFactory.getLogger(GameShareInfo.class);

    /** The casino. */
    @Column(nullable = false)
    @NotNull
    private @Getter String casino = StringUtils.EMPTY;

    /** The cabinet Type. */
    @Column(nullable = false)
    @NotNull
    private @Getter String cabinetType = StringUtils.EMPTY;

    /** The game title. */
    @Column(nullable = false)
    @NotNull
    private @Getter String gameTitle;

    /**
     * The time (milliseconds since epoch) that the game-play occurred.
     */
    @Column(nullable = false)
    @NotNull
    private @Getter Long gamePlayedAt;

    /** The cents won. */
    @Column(nullable = false)
    @NotNull
    private @Getter Integer centsWon;

    /** The game recall data. */
    @Column(columnDefinition = "text", nullable = false)
    @NotNull
    private @Getter String gameRecallData;

    /**
     * Instantiates a new game share info.
     *
     * @param casino         the casino
     * @param cabinetType    the cabinet type, atlas or retro
     * @param gameTitle      the game title
     * @param centsWon       the cents won
     * @param gameRecallData the game recall data
     */
    GameShareInfo(final Long gamePlayedAt, final String casino, final String cabinetType, final String gameTitle,
            final Integer centsWon, final String gameRecallData) {
        if (!StringUtils.isBlank(casino)) {
            this.casino = StringUtils.trim(casino);
        } else {
            this.casino = StringUtils.EMPTY;
        }
        this.gameTitle = StringUtils.trim(gameTitle);
        this.centsWon = centsWon;
        this.gameRecallData = gameRecallData;
        this.gamePlayedAt = gamePlayedAt;

        if (!StringUtils.isBlank(cabinetType)) {
            this.cabinetType = StringUtils.trim(cabinetType);
        } else {
            this.cabinetType = StringUtils.EMPTY;
        }
    }
}
