/*
 * Copyright (c) 2016 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.config;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

import lombok.Getter;
import lombok.Setter;

/**
 * The Class JasyptFixedSaltEncryptorProperties.
 */
@Component
@ConfigurationProperties(prefix = "data-encryptor")
public class JasyptFixedSaltEncryptorProperties {

    /** The salt. */
    private @Getter @Setter String salt;

    /** The password. */
    private @Getter @Setter String password;

    /** The algorithm. */
    private @Getter @Setter String algorithm;

    /** The key obtention iterations. */
    private @Getter @Setter int keyObtentionIterations;

    /** The pool size. */
    private @Getter @Setter int poolSize;

    /** The provider name. */
    private @Getter @Setter String providerName;
}
