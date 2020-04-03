/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.model;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.junit.jupiter.SpringExtension;

@ExtendWith(SpringExtension.class)
@SpringBootTest
public class GameShareInfoTest {

	@Test
	public void testStringCtor() {
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), "bummer, dude. Better luck next time.");
		assertNotNull(gsInfo);
	}

	@Test
	public void testGetGameTitle() {
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), "bummer, dude. Better luck next time.");
		assertEquals("New Money", gsInfo.getGameTitle());
	}

	@Test
	public void testGetCentsWon() {
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), "bummer, dude. Better luck next time.");
		assertEquals(Integer.valueOf(12), gsInfo.getCentsWon());
	}

	@Test
	public void testGetGameRecallData() {
		final String gameRecallData = "bummer, dude. Better luck next time.";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), gameRecallData);
		assertEquals(gameRecallData, gsInfo.getGameRecallData());
	}

	@Test
	public void testGetCasino() {
		final String casino = "Lucky Star";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), "bummer, dude. Better luck next time.");
		assertEquals(casino, gsInfo.getCasino());
	}

	@Test
	public void testGetCabinetType() {
		final String cabinetType = "Atlas";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), "bummer, dude. Better luck next time.");
		assertEquals(cabinetType, gsInfo.getCabinetType());
	}

	@Test
	public void testGetGamePlayedAt() {
		final Long playedAt = Long.valueOf(1461870653220L);
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), "bummer, dude. Better luck next time.");
		assertEquals(playedAt, gsInfo.getGamePlayedAt());
	}

	@Test
	public void testObjEquality() {
		final String gameRecallData = "bummer, dude. Better luck next time.";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), gameRecallData);
		final GameShareInfo gsInfo2 = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas",
				"New Money", Integer.valueOf(12), gameRecallData);
		assertEquals(gsInfo, gsInfo2);
	}

	@Test
	public void testObjInequalityGameTitle() {
		final String gameRecallData = "bummer, dude. Better luck next time.";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), gameRecallData);
		final GameShareInfo gsInfo2 = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas",
				"Pink Sapphires", Integer.valueOf(12), gameRecallData);
		assertFalse(gsInfo.equals(gsInfo2));
	}

	@Test
	public void testObjInequalityCentsWon() {
		final String gameRecallData = "bummer, dude. Better luck next time.";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), gameRecallData);
		final GameShareInfo gsInfo2 = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas",
				"New Money", Integer.valueOf(13), gameRecallData);
		assertFalse(gsInfo.equals(gsInfo2));
	}

	@Test
	public void testObjInequalityGameRecall() {
		final String gameRecallData = "bummer, dude. Better luck next time.";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), gameRecallData);
		final GameShareInfo gsInfo2 = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas",
				"New Money", Integer.valueOf(12), gameRecallData + "  Or Not!");
		assertFalse(gsInfo.equals(gsInfo2));
	}

	@Test
	public void testObjInequalityCasino() {
		final String gameRecallData = "bummer, dude. Better luck next time.";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), gameRecallData);
		final GameShareInfo gsInfo2 = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Galaxy", "Atlas",
				"New Money", Integer.valueOf(12), gameRecallData);
		assertFalse(gsInfo.equals(gsInfo2));
	}

	@Test
	public void testObjInequalityCabinetType() {
		final String gameRecallData = "bummer, dude. Better luck next time.";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), gameRecallData);
		final GameShareInfo gsInfo2 = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Galaxy", "Retros",
				"New Money", Integer.valueOf(12), gameRecallData);
		assertFalse(gsInfo.equals(gsInfo2));
	}

	@Test
	public void testObjInequalityGamePlayedAt() {
		final String gameRecallData = "bummer, dude. Better luck next time.";
		final GameShareInfo gsInfo = new GameShareInfo(Long.valueOf(1461870653220L), "Lucky Star", "Atlas", "New Money",
				Integer.valueOf(12), gameRecallData);
		final GameShareInfo gsInfo2 = new GameShareInfo(Long.valueOf(1461870643220L), "Lucky Galaxy", "Atlas",
				"New Money", Integer.valueOf(12), gameRecallData);
		assertFalse(gsInfo.equals(gsInfo2));
	}
}
