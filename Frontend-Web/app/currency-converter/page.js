import Head from 'next/head';
import CurrencyConverter from './CurrencyConverter';
import styles from './CurrencyConverterPage.module.css';
import WeatherPopup from './Weather';


export default function CurrencyConverterPage() {
  return (
    <div className={styles.pageContainer}>
        <title>Currency Converter</title>
        <meta name="description" content="Convert between world currencies" />
        <CurrencyConverter />
        <WeatherPopup />
      </div>

  );
}