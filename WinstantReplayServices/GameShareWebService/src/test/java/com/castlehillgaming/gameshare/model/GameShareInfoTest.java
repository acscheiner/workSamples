/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.model;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertNotNull;

import org.junit.Test;

public class GameShareInfoTest {

    @Test
    public void testStringCtor() {
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, "bummer, dude. Better luck next time.");
        assertNotNull(gsInfo);
    }

    @Test
    public void testGetGameTitle() {
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, "bummer, dude. Better luck next time.");
        assertEquals("New Money", gsInfo.getGameTitle());
    }

    @Test
    public void testGetCentsWon() {
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, "bummer, dude. Better luck next time.");
        assertEquals(Integer.valueOf(12), gsInfo.getCentsWon());
    }

    @Test
    public void testGetGameRecallData() {
        final String gameRecallData = "bummer, dude. Better luck next time.";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, gameRecallData);
        assertEquals(gameRecallData, gsInfo.getGameRecallData());
    }

    @Test
    public void testGetCasino() {
        final String casino = "Lucky Star";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, "bummer, dude. Better luck next time.");
        assertEquals(casino, gsInfo.getCasino());
    }

    @Test
    public void testGetCabeinetType() {
        final String cabinetType = "Atlas";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, "bummer, dude. Better luck next time.");
        assertEquals(cabinetType, gsInfo.getCabinetType());
    }

    @Test
    public void testGetGamePlayedAt() {
        final Long playedAt = 1461870653220L;
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, "bummer, dude. Better luck next time.");
        assertEquals(playedAt, gsInfo.getGamePlayedAt());
    }

    @Test
    public void testObjEquality() {
        final String gameRecallData = "bummer, dude. Better luck next time.";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, gameRecallData);
        final GameShareInfo gsInfo2 = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas",
                "New Money", 12, gameRecallData);
        assertEquals(gsInfo, gsInfo2);
    }

    @Test
    public void testObjInequalityGameTitle() {
        final String gameRecallData = "bummer, dude. Better luck next time.";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, gameRecallData);
        final GameShareInfo gsInfo2 = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas",
                "Pink Sapphires", 12, gameRecallData);
        assertFalse(gsInfo.equals(gsInfo2));
    }

    @Test
    public void testObjInequalityCentsWon() {
        final String gameRecallData = "bummer, dude. Better luck next time.";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, gameRecallData);
        final GameShareInfo gsInfo2 = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas",
                "New Money", 13, gameRecallData);
        assertFalse(gsInfo.equals(gsInfo2));
    }

    @Test
    public void testObjInequalityGameRecall() {
        final String gameRecallData = "bummer, dude. Better luck next time.";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, gameRecallData);
        final GameShareInfo gsInfo2 = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas",
                "New Money", 12, gameRecallData + "  Or Not!");
        assertFalse(gsInfo.equals(gsInfo2));
    }

    @Test
    public void testObjInequalityCasino() {
        final String gameRecallData = "bummer, dude. Better luck next time.";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, gameRecallData);
        final GameShareInfo gsInfo2 = new GameShareInfo(1461870653220L, "Lucky Galaxy", "Atlas",
                "New Money", 12, gameRecallData);
        assertFalse(gsInfo.equals(gsInfo2));
    }

    @Test
    public void testObjInequalityCabinetType() {
        final String gameRecallData = "bummer, dude. Better luck next time.";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, gameRecallData);
        final GameShareInfo gsInfo2 = new GameShareInfo(1461870653220L, "Lucky Galaxy", "Retros",
                "New Money", 12, gameRecallData);
        assertFalse(gsInfo.equals(gsInfo2));
    }

    @Test
    public void testObjInequalityGamePlayedAt() {
        final String gameRecallData = "bummer, dude. Better luck next time.";
        final GameShareInfo gsInfo = new GameShareInfo(1461870653220L, "Lucky Star", "Atlas", "New Money",
                12, gameRecallData);
        final GameShareInfo gsInfo2 = new GameShareInfo(1461870643220L, "Lucky Galaxy", "Atlas",
                "New Money", 12, gameRecallData);
        assertFalse(gsInfo.equals(gsInfo2));
    }
}
