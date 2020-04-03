/*
 * Copyright (c) 2016 Castle Hill Gaming, LLC. All rights reserved.
 */

//////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Use this @TypeDefs annotation for production build (encrypted data field)
//@TypeDefs({ @TypeDef(name = com.castlehillgaming.gameshare.model.Ticket.ENCRYPTED_STRING_TYPENAME, typeClass = EncryptedStringType.class, parameters = {
//        @Parameter(name = "encryptorRegisteredName", value = com.castlehillgaming.gameshare.config.EncryptionConfig.REGISTERED_NAME) }) })
//////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Use this @TypeDefs annotation for development build (clear text data field)
@TypeDefs({
        @TypeDef(name = com.castlehillgaming.gameshare.model.Ticket.ENCRYPTED_STRING_TYPENAME, typeClass = String.class, parameters = {
                @Parameter(name = "encryptorRegisteredName", value = com.castlehillgaming.gameshare.config.EncryptionConfig.REGISTERED_NAME) }) })
//////////////////////////////////////////////////////////////////////////////////////////////////////////////

package com.castlehillgaming.gameshare.model;

import org.hibernate.annotations.Parameter;
import org.hibernate.annotations.TypeDef;
import org.hibernate.annotations.TypeDefs;
