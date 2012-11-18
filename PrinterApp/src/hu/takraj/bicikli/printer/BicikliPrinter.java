/**
 * 
 */
package hu.takraj.bicikli.printer;

import hu.takraj.bicikli.printer.models.InvoiceModel;
import hu.takraj.bicikli.printer.models.PrinterModel;

import java.awt.print.PrinterJob;
import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.URL;
import java.nio.charset.Charset;
import java.util.Calendar;
import java.util.Properties;
import java.util.Timer;
import java.util.TimerTask;

import com.google.gson.Gson;

/**
 * @author TakRaj
 * 
 */
public class BicikliPrinter {

	private static String propertiesFileName = "config.properties";
	private static BicikliPrinterConfig config = new BicikliPrinterConfig();

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		try {
			loadProperties();
			announce();
			listen();
		} catch (FileNotFoundException ex) {
			log("HIBA: Nem talalom a " + propertiesFileName + " fajlt!");
		} catch (IOException ex) {
			log("HIBA: A " + propertiesFileName + " fajl nem elerheto!");
		} catch (NumberFormatException ex) {
			log("HIBA: Hibas szamformatum talalhato a " + propertiesFileName
					+ " fajlban.");
		} catch (IllegalArgumentException ex) {
			log("HIBA: A " + propertiesFileName
					+ " hibas parameterekkel rendelkezik!");
		} catch (Throwable t) {
			log("HIBA: Altalanos hiba tortent -> "
					+ t.getClass().getCanonicalName());
		}
	}

	private static void announce() {

		final Timer timer = new Timer(true);

		Runtime.getRuntime().addShutdownHook(new Thread(new Runnable() {

			@Override
			public void run() {
				try {
					log("INFO: Leallitas... nyomtato kiregisztralasa...");
					timer.cancel();
					unregisterPrinter();
				} catch (Throwable t) {
					log("HIBA: Nem sikerult a nyomtatot eltavolitani az adatbazisbol!");
				} finally {
					System.out.println("--- A nyomtatoszerver leallt. ---");
				}
			}

		}));
		timer.scheduleAtFixedRate(new TimerTask() {

			@Override
			public void run() {
				try {
					log("BEJELENTES: url=" + config.announceUrl + ", ip="
							+ config.localIP + ", port=" + config.localPort);
					registerPrinter();
				} catch (Throwable t) {
					log("SIKERTELEN BEJELENTES! Ellenorizze az adatokat!");
				}
			}

		}, 30, config.announceInterval);
	}

	private static void loadProperties() throws Throwable {
		Properties prop = new Properties();
		prop.load(new FileInputStream(propertiesFileName));

		config.announceUrl = prop.getProperty("remote.announce.url");
		config.announceInterval = Integer.parseInt(prop
				.getProperty("remote.announce.interval"));
		config.localIP = prop.getProperty("local.ip");
		config.localPort = 6060; // nem akarom tulbonyolitani... :)
		config.lenderId = Integer.parseInt(prop.getProperty("local.lender.id"));
		config.printerPassword = prop.getProperty("remote.printer.password");
	}

	private static void listen() {
		try {
			ServerSocket socket = new ServerSocket(config.localPort);
			System.out
					.println("--- Nyomtató szerver elinditva a kovetkezo porton: "
							+ socket.getLocalPort() + " ---");
			while (true) {
				final Socket connection = socket.accept();
				log("INFO: Kapcsolodasi keres erkezett a kovetkezo helyrol: "
						+ connection.getInetAddress().getCanonicalHostName());
				new Thread(new Runnable() {
					@Override
					public void run() {
						try {
							handlePrinting(connection.getInputStream());
							log("INFO: Kapcsolat lezarva a kovetkezovel: "
									+ connection.getInetAddress()
											.getCanonicalHostName());
						} catch (Throwable t) {
							log("HIBA: Nem sikerult fogadni a kapcsolodasi kerest a kovetkezo helyrol: "
									+ connection.getInetAddress()
											.getCanonicalHostName());
						}
					}

				}).start();
			}
		} catch (Throwable t) {
			log("HIBA: Kritikus hiba tortent. Tovabbi adatok fogadasa nem lehetseges. -> "
					+ t.getClass().getCanonicalName());
		}
	}

	private static void handlePrinting(InputStream inputStream) {
		try {
			StringBuilder jsonData = new StringBuilder();

			String line = null;
			BufferedReader reader = new BufferedReader(new InputStreamReader(
					inputStream, Charset.forName("UTF-8").newDecoder()));
			while ((line = reader.readLine()) != null) {
				jsonData.append(line);
			}

			Gson gson = new Gson();
			InvoiceModel invoice = gson.fromJson(jsonData.toString(),
					InvoiceModel.class);

			log("INFO: Nyomtatasi feladat kuldese...");
			try {
				PrinterJob job = PrinterJob.getPrinterJob();
				job.setPrintable(new InvoicePrinter(invoice));
				job.print();
				log("INFO: Nyomtatási feladat sikeresen elkuldve.");
			} catch (Throwable t) {
				log("HIBA: A nyomtatas sikertelen. -> "
						+ t.getClass().getCanonicalName());
			}
		} catch (Throwable t) {
			log("HIBA: Az adat fogadasa sikertelen volt. -> "
					+ t.getClass().getCanonicalName());
		} finally {
			try {
				inputStream.close();
			} catch (Throwable t) {
				// dummy
			}
		}
	}

	private static void log(String msg) {
		System.out.println(Calendar.getInstance().getTime() + " | " + msg);
	}

	private static void registerPrinter() throws Throwable {
		URL url = new URL(config.announceUrl);
		HttpURLConnection httpCon = (HttpURLConnection) url.openConnection();

		httpCon.setDoOutput(true);
		httpCon.setRequestMethod("PUT");
		httpCon.setRequestProperty("Content-Type", "application/json");
		httpCon.setRequestProperty("charset", "UTF-8");
		httpCon.connect();
		OutputStream out = httpCon.getOutputStream();

		Gson gson = new Gson();
		out.write(gson
				.toJson(new PrinterModel(config.lenderId, config.localIP, config.printerPassword))
				.getBytes("UTF-8"));
		out.flush();
		out.close();
		if (httpCon.getResponseCode() == HttpURLConnection.HTTP_FORBIDDEN) {
			log("Sikertelen bejelentes: Rossz jelszo.");
		}
		httpCon.disconnect();
	}

	private static void unregisterPrinter() throws Throwable {
		URL url = new URL(config.announceUrl);
		HttpURLConnection httpCon = (HttpURLConnection) url.openConnection();

		httpCon.setDoOutput(true);
		httpCon.setRequestMethod("DELETE");
		httpCon.setRequestProperty("Content-Type", "application/json");
		httpCon.setRequestProperty("charset", "UTF-8");
		httpCon.connect();
		OutputStream out = httpCon.getOutputStream();

		Gson gson = new Gson();
		out.write(gson.toJson(config.lenderId).getBytes("UTF-8"));
		out.flush();
		out.close();
		httpCon.getResponseCode();
		httpCon.disconnect();
	}

}
