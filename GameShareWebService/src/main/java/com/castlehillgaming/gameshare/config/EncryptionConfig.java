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
import org.springframework.core.env.Environment;

/**
 * The Class EncryptionConfig.
 */
@Configuration
public class EncryptionConfig {

    /** The Constant logger. */
    @SuppressWarnings("unused")
    private static final Logger logger = LoggerFactory.getLogger(EncryptionConfig.class);

    /** The Constant DEFAULT_PROVIDER_NAME. */
    private static final String DEFAULT_PROVIDER_NAME = "BC";

    /** The Constant DEFAULT_POOL_SIZE. */
    private static final String DEFAULT_POOL_SIZE = "8";

    public static final String REGISTERED_NAME = "strongFixedSaltStringEncryptor";

    /** The Spring Environment. */
    @Autowired
    private Environment env;

    /**
     * Fixed string salt generator.
     *
     * @return the salt generator
     */
    @Bean
    public SaltGenerator fixedSaltGenerator() {
        if (!env.containsProperty("jasypt.fixedSaltStringEncryptor.salt")) {
            return null;
        }

        final String salt = env.getProperty("jasypt.fixedSaltStringEncryptor.salt");
        return new StringFixedSaltGenerator(salt);
    }

    /**
     * Hibernate fixed salt string encryptor.
     *
     * @return the hibernate PBE string encryptor
     */
    @Bean
    public HibernatePBEStringEncryptor hibernateFixedSaltStringEncryptor() {
        if (!env.containsProperty("jasypt.fixedSaltStringEncryptor.algorithm")
                || !env.containsProperty("jasypt.fixedSaltStringEncryptor.keyObtentionIterations")
                || !env.containsProperty("jasypt.fixedSaltStringEncryptor.password") || null == fixedSaltGenerator()) {
            return null;
        }

        final HibernatePBEStringEncryptor hibernateStringEncryptor = new HibernatePBEStringEncryptor();

        final EnvironmentStringPBEConfig encConfig = new EnvironmentStringPBEConfig();
        encConfig.setProviderName(
                env.getProperty("jasypt.fixedSaltStringEncryptor.providerName", DEFAULT_PROVIDER_NAME));
        encConfig.setAlgorithm(env.getProperty("jasypt.fixedSaltStringEncryptor.algorithm"));
        encConfig.setKeyObtentionIterations(env.getProperty("jasypt.fixedSaltStringEncryptor.keyObtentionIterations"));
        encConfig.setPassword(env.getProperty("jasypt.fixedSaltStringEncryptor.password"));
        encConfig.setPoolSize(env.getProperty("jasypt.fixedSaltStringEncryptor.poolSize", DEFAULT_POOL_SIZE));

        final PooledPBEStringEncryptor encryptor = new PooledPBEStringEncryptor();
        encryptor.setConfig(encConfig);
        encryptor.setSaltGenerator(fixedSaltGenerator());

        hibernateStringEncryptor.setRegisteredName(REGISTERED_NAME);
        hibernateStringEncryptor.setEncryptor(encryptor);

        return hibernateStringEncryptor;
    }
}
