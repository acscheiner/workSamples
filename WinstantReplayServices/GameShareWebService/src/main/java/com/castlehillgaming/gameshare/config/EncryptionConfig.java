/*
 * Copyright (c) 2016 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare.config;

import org.jasypt.encryption.pbe.PooledPBEStringEncryptor;
import org.jasypt.encryption.pbe.config.EnvironmentStringPBEConfig;
import org.jasypt.hibernate4.encryptor.HibernatePBEStringEncryptor;
import org.jasypt.salt.SaltGenerator;
import org.jasypt.salt.StringFixedSaltGenerator;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Profile;

/**
 * The Class EncryptionConfig.
 */
@Configuration
@Profile("dataencrypt")
public class EncryptionConfig {

    /** The Constant logger. */
    @SuppressWarnings("unused")
    private static final Logger logger = LoggerFactory.getLogger(EncryptionConfig.class);

    /** The Constant REGISTERED_NAME. */
    public static final String REGISTERED_NAME = "strongFixedSaltStringEncryptor";

    /** The jfse props. */
    @Autowired
    private JasyptFixedSaltEncryptorProperties jfseProps;

    /**
     * Fixed string salt generator.
     *
     * @return the salt generator
     */
    @Bean
    public SaltGenerator fixedSaltGenerator() {
        return new StringFixedSaltGenerator(jfseProps.getSalt());
    }

    /**
     * Hibernate fixed salt string encryptor.
     *
     * @return the hibernate PBE string encryptor
     */
    @Bean
    public HibernatePBEStringEncryptor hibernateFixedSaltStringEncryptor() {
        final HibernatePBEStringEncryptor hibernateStringEncryptor = new HibernatePBEStringEncryptor();

        final EnvironmentStringPBEConfig encConfig = new EnvironmentStringPBEConfig();
        encConfig.setProviderName(jfseProps.getProviderName());
        encConfig.setAlgorithm(jfseProps.getAlgorithm());
        encConfig.setKeyObtentionIterations(jfseProps.getKeyObtentionIterations());
        encConfig.setPassword(jfseProps.getPassword());
        encConfig.setPoolSize(jfseProps.getPoolSize());

        final PooledPBEStringEncryptor encryptor = new PooledPBEStringEncryptor();
        encryptor.setConfig(encConfig);
        encryptor.setSaltGenerator(fixedSaltGenerator());

        hibernateStringEncryptor.setRegisteredName(REGISTERED_NAME);
        hibernateStringEncryptor.setEncryptor(encryptor);

        return hibernateStringEncryptor;
    }
}
