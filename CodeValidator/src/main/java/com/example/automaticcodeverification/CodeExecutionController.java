package com.example.automaticcodeverification;

import org.springframework.data.domain.Example;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;

import javax.tools.*;
import java.io.ByteArrayOutputStream;
import java.io.OutputStream;
import java.util.Arrays;
import java.util.logging.Logger;

@RestController
@RequestMapping("/handleJsonRequest")
public class CodeExecutionController {

    private static final Logger LOGGER = Logger.getLogger(Example.class.getName());

    @GetMapping(produces = MediaType.APPLICATION_JSON_VALUE)
    public String handleJsonRequest(@RequestBody String code) {
        try {
            // Это предположительный код для компиляции и выполнения кода Java
            JavaCompiler compiler = ToolProvider.getSystemJavaCompiler();
            DiagnosticCollector<JavaFileObject> diagnostics = new DiagnosticCollector<>();
            StandardJavaFileManager fileManager = compiler.getStandardFileManager(diagnostics, null, null);

            // Создание временного файла для сохранения кода
            JavaFileObject file = new MyJavaFileObject("DynamicCode", code);
            Iterable<? extends JavaFileObject> compilationUnits = Arrays.asList(file);
            String[] compileOptions = new String[]{"-d", "compiled"};

            // Компиляция исходного кода
            JavaCompiler.CompilationTask task = compiler.getTask(null, fileManager, diagnostics, Arrays.asList(compileOptions), null, compilationUnits);
            boolean success = task.call();

            if (success) {
                // Выполнение скомпилированного кода
                //ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
                ClassLoader classLoader = fileManager.getClassLoader(StandardLocation.CLASS_OUTPUT);
                Class<?> dynamicClass = classLoader.loadClass("DynamicCode");
                Runnable runnable = (Runnable) dynamicClass.newInstance();
                runnable.run();
                return "Code executed successfully!";
            } else {
                // Обработка ошибок компиляции, если они возникли
                StringBuilder errors = new StringBuilder();
                for (Diagnostic<? extends JavaFileObject> diagnostic : diagnostics.getDiagnostics()) {
                    errors.append(diagnostic.getMessage(null)).append("\n");
                }
                return "Code compilation failed with errors:\n" + errors.toString();
            }
        } catch (Exception e) {
            LOGGER.severe("Произошло исключение: " + e.getMessage());
            e.printStackTrace();
        }
        return "Code executed successfully!";
    }
}
