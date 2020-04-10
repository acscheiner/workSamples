/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare;

import java.security.Security;
import java.util.TimeZone;

import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.jms.annotation.EnableJms;

import com.castlehillgaming.gameshare_commonutils.SharedConstants;

/**
 * The Class GameShareServiceApplication. Main class which bootstraps this
 * SpringBoot based web application.
 */
@SpringBootApplication
@EnableJpaRepositories(basePackages = "com.castlehillgaming.gameshare.dao")
@EntityScan(basePackages = "com.castlehillgaming.gameshare.model")
@EnableJms
public class GameShareServiceApplication {

    /**
     * The main method.
     *
     * @param args the arguments
     */
    public static void main(final String[] args) {
        TimeZone.setDefault(SharedConstants.utc);
        Security.addProvider(new BouncyCastleProvider());
        SpringApplication.run(GameShareServiceApplication.class, args);
    }
}
