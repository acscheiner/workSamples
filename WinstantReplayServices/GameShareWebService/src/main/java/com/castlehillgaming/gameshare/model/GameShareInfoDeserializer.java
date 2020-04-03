/*
 * Copyright (c) 2016 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.model;

import java.io.IOException;

import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.ObjectCodec;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.JsonDeserializer;
import com.fasterxml.jackson.databind.JsonNode;

/**
 * The Class GameShareInfoDeserializer provides custom JSON deserialization for
 * binding Game-Share web request body JSON data into a GameShareInfo object.
 * Specifically, we need to map the gameTitle to an instance of GameTitleEnum
 * using the custom filtering/mapping in the GameTitleEnum.getInstance method.
 */
public class GameShareInfoDeserializer extends JsonDeserializer<GameShareInfo> {

    /*
     * (non-Javadoc)
     *
     * @see
     * com.fasterxml.jackson.databind.JsonDeserializer#deserialize(com.fasterxml
     * .jackson.core.JsonParser,
     * com.fasterxml.jackson.databind.DeserializationContext)
     */
    @Override
    public GameShareInfo deserialize(final JsonParser jsonParser, final DeserializationContext deserializationContext)
            throws IOException, JsonProcessingException {
        final ObjectCodec oc = jsonParser.getCodec();
        final JsonNode node = oc.readTree(jsonParser);
        final String casino = node.get("casino").asText();
        final String cabinetType = node.get("cabinetType").asText();
        final String gameTitle = node.get("gameTitle").asText();
        final Long gamePlayedAt = node.get("playedAt").asLong();
        final Integer centsWon = node.get("centsWon").asInt();
        final String gameRecallData = node.get("gameRecallData").asText();
        return new GameShareInfo(gamePlayedAt, casino, cabinetType, gameTitle, centsWon, gameRecallData);
    }
}
