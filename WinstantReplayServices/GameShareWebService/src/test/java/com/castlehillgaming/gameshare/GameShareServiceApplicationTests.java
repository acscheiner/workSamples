/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare;

import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.context.annotation.Profile;
import org.springframework.test.context.junit4.SpringRunner;
import org.springframework.test.context.web.WebAppConfiguration;

/**
 * The Class GameShareServiceApplicationTests.
 */
@RunWith(SpringRunner.class)
@SpringBootTest(classes = GameShareServiceApplication.class)
@WebAppConfiguration
@Profile("development")
public class GameShareServiceApplicationTests {

    /**
     * Context loads.
     */
    @Test
    public void contextLoads() {
    }

}
