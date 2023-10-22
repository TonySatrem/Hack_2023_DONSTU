package com.example.automaticcodeverification;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

public class ControllerTest {

    private String reverseString(String input) {
        return new StringBuilder(input).reverse().toString();
    }

    @Test
    public void testReverseString() {
        assertEquals("olleH", reverseString("Hello"));
    }


}
