package com.example.automaticcodeverification;

import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;

@RestController
public class Controller {

   @GetMapping(path = "/example", consumes = MediaType.APPLICATION_JSON_VALUE)
    public String handleJsonRequest(@RequestBody String jsonRequest) {
        // Здесь вы можете обрабатывать полученный JSON-запрос
        System.out.println("Received JSON request: " + jsonRequest);

        // Верните ответ в виде строки или объекта JSON, в зависимости от вашей логики обработки запроса
        return "Response from server" + jsonRequest;
    }

    /*@GetMapping(path = "/cors")
    public String handleJson(@RequestParam String currentDirectory) {
            currentDirectory = System.getProperty("user.dir");
            return "Текущая директория: " + currentDirectory;
    }*/
}
