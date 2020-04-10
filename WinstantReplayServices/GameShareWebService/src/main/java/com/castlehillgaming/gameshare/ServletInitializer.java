/*
 * Copyright (c) 2015 Castle Hill Gaming, LLC. All rights reserved.
 */
package com.castlehillgaming.gameshare;

import org.springframework.boot.builder.SpringApplicationBuilder;
import org.springframework.boot.web.servlet.support.SpringBootServletInitializer;

/**
 * The Class ServletInitializer loads configuration information for this
 * SpringBoot web application.
 */
public class ServletInitializer extends SpringBootServletInitializer {

    /*
     * (non-Javadoc)
     *
     * @see org.springframework.boot.context.web.SpringBootServletInitializer#
     * configure (org.springframework.boot.builder.SpringApplicationBuilder)
     */
    @Override
    protected SpringApplicationBuilder configure(final SpringApplicationBuilder application) {
        return application.sources(GameShareServiceApplication.class);
    }

}
