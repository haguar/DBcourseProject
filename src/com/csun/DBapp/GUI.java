package com.csun.DBapp;

import java.util.Scanner;

public class GUI {
    private String command;
    public void startGUI() {
        System.out.println("DBapp menu: \n");
        Scanner scanner = new Scanner(System.in);
        help();
        do {
            command = scanner.nextLine();
        } while (command != "exit");
    }
    private void help () {

    }
}

