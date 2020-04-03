/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare;

import java.io.File;

import org.jasypt.encryption.pbe.PooledPBEStringEncryptor;
import org.jasypt.encryption.pbe.config.SimpleStringPBEConfig;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.junit4.SpringRunner;
import org.springframework.test.context.web.WebAppConfiguration;

/**
 * The Class GameShareServiceApplicationTests.
 */
@RunWith(SpringRunner.class)
@SpringBootTest(classes = GameShareServiceApplication.class)
@WebAppConfiguration
public class GameShareServiceApplicationTests {

    static {
        final PooledPBEStringEncryptor encryptor = new PooledPBEStringEncryptor();
        final SimpleStringPBEConfig pbeConfig = new SimpleStringPBEConfig();
        pbeConfig.setPassword(System.getenv("CHG_PBE_ENC"));
        pbeConfig.setAlgorithm("PBEWITHSHA256AND256BITAES-CBC-BC");
        pbeConfig.setKeyObtentionIterations("84691");
        pbeConfig.setPoolSize("1");
        pbeConfig.setProviderName("BC");
        pbeConfig.setSaltGeneratorClassName("org.jasypt.salt.RandomSaltGenerator");
        pbeConfig.setStringOutputType("base64");
        encryptor.setConfig(pbeConfig);

        System.setProperty("javax.net.ssl.trustStore",
                System.getenv("CHG_WRSVC_KEYS") + File.separator + "amq-webservice-client.ts");
        System.setProperty("javax.net.ssl.trustStorePassword",
                encryptor.decrypt("mbaSjfcuwi3zKLFA6CHhZ4Lh/chRc3rYX7+RAxC9DyE="));
        System.setProperty("javax.net.ssl.keyStore",
                System.getenv("CHG_WRSVC_KEYS") + File.separator + "amq-webservice-client.ks");
        System.setProperty("javax.net.ssl.keyStorePassword",
                encryptor.decrypt("SDNoLi4lcEYsEk1XRMgmX32FKtCY/kvGlCfuEAEJzmc="));
    }

    /**
     * Context loads.
     */
    @Test
    public void contextLoads() {
    }

}
