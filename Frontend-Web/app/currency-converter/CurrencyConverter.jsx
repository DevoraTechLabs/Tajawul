"use client";
import { useState, useEffect } from "react";
import styles from "./CurrencyConverterPage.module.css";
import API from "@/utils/api";
import axios from "axios";
import SwapHorizIcon from "@mui/icons-material/SwapHoriz";
import { motion } from "framer-motion";

const currencyCodes = [
    "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN",
    "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL",
    "BSD", "BTC", "BTN", "BWP", "BYN", "BZD", "CAD", "CDF", "CHF", "CLP",
    "CNY", "COP", "CRC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD",
    "EGP", "ERN", "ETB", "EUR", "FJD", "FKP", "GBP", "GEL", "GHS", "GIP",
    "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR",
    "ILS", "INR", "IQD", "IRR", "ISK", "JMD", "JOD", "JPY", "KES", "KGS",
    "KHR", "KMF", "KPW", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR",
    "LRD", "LSL", "LYD", "MAD", "MDL", "MGA", "MKD", "MMK", "MNT", "MOP",
    "MRU", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO",
    "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN",
    "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SDG",
    "SEK", "SGD", "SHP", "SLL", "SOS", "SRD", "SSP", "STN", "SYP", "SZL",
    "THB", "TJS", "TMT", "TND", "TOP", "TRY", "TTD", "TWD", "TZS", "UAH",
    "UGX", "USD", "UYU", "UZS", "VES", "VND", "VUV", "WST", "XAF", "XCD",
    "XOF", "XPF", "YER", "ZAR", "ZMW", "ZWL"
];

const currencySymbols = {
    "AED": "د.إ",      // UAE Dirham
    "AFN": "؋",        // Afghan Afghani
    "ALL": "L",        // Albanian Lek
    "AMD": "֏",        // Armenian Dram
    "ANG": "ƒ",        // Netherlands Antillean Guilder
    "AOA": "Kz",       // Angolan Kwanza
    "ARS": "$",        // Argentine Peso
    "AUD": "$",        // Australian Dollar
    "AWG": "ƒ",        // Aruban Florin
    "AZN": "₼",        // Azerbaijani Manat
    "BAM": "KM",       // Bosnia-Herzegovina Convertible Mark
    "BBD": "$",        // Barbadian Dollar
    "BDT": "৳",        // Bangladeshi Taka
    "BGN": "лв",       // Bulgarian Lev
    "BHD": ".د.ب",     // Bahraini Dinar
    "BIF": "FBu",      // Burundian Franc
    "BMD": "$",        // Bermudan Dollar
    "BND": "$",        // Brunei Dollar
    "BOB": "$b",       // Bolivian Boliviano
    "BRL": "R$",       // Brazilian Real
    "BSD": "$",        // Bahamian Dollar
    "BTC": "₿",        // Bitcoin
    "BTN": "Nu.",      // Bhutanese Ngultrum
    "BWP": "P",        // Botswanan Pula
    "BYN": "Br",       // Belarusian Ruble
    "BZD": "BZ$",      // Belize Dollar
    "CAD": "$",        // Canadian Dollar
    "CDF": "FC",       // Congolese Franc
    "CHF": "CHF",      // Swiss Franc
    "CLP": "$",        // Chilean Peso
    "CNY": "¥",        // Chinese Yuan
    "COP": "$",        // Colombian Peso
    "CRC": "₡",        // Costa Rican Colón
    "CUP": "₱",        // Cuban Peso
    "CVE": "$",        // Cape Verdean Escudo
    "CZK": "Kč",       // Czech Koruna
    "DJF": "Fdj",      // Djiboutian Franc
    "DKK": "kr",       // Danish Krone
    "DOP": "RD$",      // Dominican Peso
    "DZD": "دج",       // Algerian Dinar
    "EGP": "£",        // Egyptian Pound
    "ERN": "Nfk",      // Eritrean Nakfa
    "ETB": "Br",       // Ethiopian Birr
    "EUR": "€",        // Euro
    "FJD": "$",        // Fijian Dollar
    "FKP": "£",        // Falkland Islands Pound
    "GBP": "£",        // British Pound Sterling
    "GEL": "₾",        // Georgian Lari
    "GHS": "₵",        // Ghanaian Cedi
    "GIP": "£",        // Gibraltar Pound
    "GMD": "D",        // Gambian Dalasi
    "GNF": "FG",       // Guinean Franc
    "GTQ": "Q",        // Guatemalan Quetzal
    "GYD": "$",        // Guyanaese Dollar
    "HKD": "$",        // Hong Kong Dollar
    "HNL": "L",        // Honduran Lempira
    "HRK": "kn",       // Croatian Kuna
    "HTG": "G",        // Haitian Gourde
    "HUF": "Ft",       // Hungarian Forint
    "IDR": "Rp",       // Indonesian Rupiah
    "ILS": "₪",        // Israeli New Shekel
    "INR": "₹",        // Indian Rupee
    "IQD": "ع.د",      // Iraqi Dinar
    "IRR": "﷼",        // Iranian Rial
    "ISK": "kr",       // Icelandic Króna
    "JMD": "J$",       // Jamaican Dollar
    "JOD": "JD",       // Jordanian Dinar
    "JPY": "¥",        // Japanese Yen
    "KES": "KSh",      // Kenyan Shilling
    "KGS": "лв",       // Kyrgyzstani Som
    "KHR": "៛",        // Cambodian Riel
    "KMF": "CF",       // Comorian Franc
    "KPW": "₩",        // North Korean Won
    "KRW": "₩",        // South Korean Won
    "KWD": "KD",       // Kuwaiti Dinar
    "KYD": "$",        // Cayman Islands Dollar
    "KZT": "₸",        // Kazakhstani Tenge
    "LAK": "₭",        // Laotian Kip
    "LBP": "£",        // Lebanese Pound
    "LKR": "₨",        // Sri Lankan Rupee
    "LRD": "$",        // Liberian Dollar
    "LSL": "L",        // Lesotho Loti
    "LYD": "LD",       // Libyan Dinar
    "MAD": "DH",       // Moroccan Dirham
    "MDL": "L",        // Moldovan Leu
    "MGA": "Ar",       // Malagasy Ariary
    "MKD": "ден",      // Macedonian Denar
    "MMK": "K",        // Myanma Kyat
    "MNT": "₮",        // Mongolian Tugrik
    "MOP": "MOP$",     // Macanese Pataca
    "MRU": "UM",       // Mauritanian Ouguiya
    "MUR": "₨",        // Mauritian Rupee
    "MVR": "Rf",       // Maldivian Rufiyaa
    "MWK": "MK",       // Malawian Kwacha
    "MXN": "$",        // Mexican Peso
    "MYR": "RM",       // Malaysian Ringgit
    "MZN": "MT",       // Mozambican Metical
    "NAD": "$",        // Namibian Dollar
    "NGN": "₦",        // Nigerian Naira
    "NIO": "C$",       // Nicaraguan Córdoba
    "NOK": "kr",       // Norwegian Krone
    "NPR": "₨",        // Nepalese Rupee
    "NZD": "$",        // New Zealand Dollar
    "OMR": "﷼",        // Omani Rial
    "PAB": "B/.",      // Panamanian Balboa
    "PEN": "S/.",      // Peruvian Nuevo Sol
    "PGK": "K",        // Papua New Guinean Kina
    "PHP": "₱",        // Philippine Peso
    "PKR": "₨",        // Pakistani Rupee
    "PLN": "zł",       // Polish Zloty
    "PYG": "Gs",       // Paraguayan Guarani
    "QAR": "﷼",        // Qatari Rial
    "RON": "lei",      // Romanian Leu
    "RSD": "Дин.",     // Serbian Dinar
    "RUB": "₽",        // Russian Ruble
    "RWF": "RWF",      // Rwandan Franc
    "SAR": "﷼",        // Saudi Riyal
    "SBD": "$",        // Solomon Islands Dollar
    "SCR": "₨",        // Seychellois Rupee
    "SDG": "ج.س.",     // Sudanese Pound
    "SEK": "kr",       // Swedish Krona
    "SGD": "$",        // Singapore Dollar
    "SHP": "£",        // Saint Helena Pound
    "SLL": "Le",       // Sierra Leonean Leone
    "SOS": "S",        // Somali Shilling
    "SRD": "$",        // Surinamese Dollar
    "SSP": "£",        // South Sudanese Pound
    "STN": "Db",       // São Tomé and Príncipe Dobra
    "SYP": "£",        // Syrian Pound
    "SZL": "L",        // Swazi Lilangeni
    "THB": "฿",        // Thai Baht
    "TJS": "SM",       // Tajikistani Somoni
    "TMT": "T",        // Turkmenistani Manat
    "TND": "DT",       // Tunisian Dinar
    "TOP": "T$",       // Tongan Paʻanga
    "TRY": "₺",        // Turkish Lira
    "TTD": "TT$",      // Trinidad and Tobago Dollar
    "TWD": "NT$",      // New Taiwan Dollar
    "TZS": "TSh",      // Tanzanian Shilling
    "UAH": "₴",        // Ukrainian Hryvnia
    "UGX": "USh",      // Ugandan Shilling
    "USD": "$",        // United States Dollar
    "UYU": "$U",       // Uruguayan Peso
    "UZS": "лв",       // Uzbekistan Som
    "VES": "Bs.S",     // Venezuelan Bolívar
    "VND": "₫",        // Vietnamese Dong
    "VUV": "VT",       // Vanuatu Vatu
    "WST": "WS$",      // Samoan Tala
    "XAF": "FCFA",     // CFA Franc BEAC
    "XCD": "$",        // East Caribbean Dollar
    "XOF": "CFA",      // CFA Franc BCEAO
    "XPF": "₣",        // CFP Franc
    "YER": "﷼",        // Yemeni Rial
    "ZAR": "R",        // South African Rand
    "ZMW": "ZK",       // Zambian Kwacha
    "ZWL": "Z$"        // Zimbabwean Dollar
};

const currencyNames = {
    "AED": "United Arab Emirates Dirham",
    "AFN": "Afghan Afghani",
    "ALL": "Albanian Lek",
    "AMD": "Armenian Dram",
    "ANG": "Netherlands Antillean Guilder",
    "AOA": "Angolan Kwanza",
    "ARS": "Argentine Peso",
    "AUD": "Australian Dollar",
    "AWG": "Aruban Florin",
    "AZN": "Azerbaijani Manat",
    "BAM": "Bosnia-Herzegovina Convertible Mark",
    "BBD": "Barbadian Dollar",
    "BDT": "Bangladeshi Taka",
    "BGN": "Bulgarian Lev",
    "BHD": "Bahraini Dinar",
    "BIF": "Burundian Franc",
    "BMD": "Bermudan Dollar",
    "BND": "Brunei Dollar",
    "BOB": "Bolivian Boliviano",
    "BRL": "Brazilian Real",
    "BSD": "Bahamian Dollar",
    "BTC": "Bitcoin",
    "BTN": "Bhutanese Ngultrum",
    "BWP": "Botswanan Pula",
    "BYN": "Belarusian Ruble",
    "BZD": "Belize Dollar",
    "CAD": "Canadian Dollar",
    "CDF": "Congolese Franc",
    "CHF": "Swiss Franc",
    "CLP": "Chilean Peso",
    "CNY": "Chinese Yuan",
    "COP": "Colombian Peso",
    "CRC": "Costa Rican Colón",
    "CUP": "Cuban Peso",
    "CVE": "Cape Verdean Escudo",
    "CZK": "Czech Republic Koruna",
    "DJF": "Djiboutian Franc",
    "DKK": "Danish Krone",
    "DOP": "Dominican Peso",
    "DZD": "Algerian Dinar",
    "EGP": "Egyptian Pound",
    "ERN": "Eritrean Nakfa",
    "ETB": "Ethiopian Birr",
    "EUR": "Euro",
    "FJD": "Fijian Dollar",
    "FKP": "Falkland Islands Pound",
    "GBP": "British Pound Sterling",
    "GEL": "Georgian Lari",
    "GHS": "Ghanaian Cedi",
    "GIP": "Gibraltar Pound",
    "GMD": "Gambian Dalasi",
    "GNF": "Guinean Franc",
    "GTQ": "Guatemalan Quetzal",
    "GYD": "Guyanaese Dollar",
    "HKD": "Hong Kong Dollar",
    "HNL": "Honduran Lempira",
    "HRK": "Croatian Kuna",
    "HTG": "Haitian Gourde",
    "HUF": "Hungarian Forint",
    "IDR": "Indonesian Rupiah",
    "ILS": "Israeli New Sheqel",
    "INR": "Indian Rupee",
    "IQD": "Iraqi Dinar",
    "IRR": "Iranian Rial",
    "ISK": "Icelandic Króna",
    "JMD": "Jamaican Dollar",
    "JOD": "Jordanian Dinar",
    "JPY": "Japanese Yen",
    "KES": "Kenyan Shilling",
    "KGS": "Kyrgystani Som",
    "KHR": "Cambodian Riel",
    "KMF": "Comorian Franc",
    "KPW": "North Korean Won",
    "KRW": "South Korean Won",
    "KWD": "Kuwaiti Dinar",
    "KYD": "Cayman Islands Dollar",
    "KZT": "Kazakhstani Tenge",
    "LAK": "Laotian Kip",
    "LBP": "Lebanese Pound",
    "LKR": "Sri Lankan Rupee",
    "LRD": "Liberian Dollar",
    "LSL": "Lesotho Loti",
    "LYD": "Libyan Dinar",
    "MAD": "Moroccan Dirham",
    "MDL": "Moldovan Leu",
    "MGA": "Malagasy Ariary",
    "MKD": "Macedonian Denar",
    "MMK": "Myanma Kyat",
    "MNT": "Mongolian Tugrik",
    "MOP": "Macanese Pataca",
    "MRU": "Mauritanian Ouguiya",
    "MUR": "Mauritian Rupee",
    "MVR": "Maldivian Rufiyaa",
    "MWK": "Malawian Kwacha",
    "MXN": "Mexican Peso",
    "MYR": "Malaysian Ringgit",
    "MZN": "Mozambican Metical",
    "NAD": "Namibian Dollar",
    "NGN": "Nigerian Naira",
    "NIO": "Nicaraguan Córdoba",
    "NOK": "Norwegian Krone",
    "NPR": "Nepalese Rupee",
    "NZD": "New Zealand Dollar",
    "OMR": "Omani Rial",
    "PAB": "Panamanian Balboa",
    "PEN": "Peruvian Nuevo Sol",
    "PGK": "Papua New Guinean Kina",
    "PHP": "Philippine Peso",
    "PKR": "Pakistani Rupee",
    "PLN": "Polish Zloty",
    "PYG": "Paraguayan Guarani",
    "QAR": "Qatari Rial",
    "RON": "Romanian Leu",
    "RSD": "Serbian Dinar",
    "RUB": "Russian Ruble",
    "RWF": "Rwandan Franc",
    "SAR": "Saudi Riyal",
    "SBD": "Solomon Islands Dollar",
    "SCR": "Seychellois Rupee",
    "SDG": "Sudanese Pound",
    "SEK": "Swedish Krona",
    "SGD": "Singapore Dollar",
    "SHP": "Saint Helena Pound",
    "SLL": "Sierra Leonean Leone",
    "SOS": "Somali Shilling",
    "SRD": "Surinamese Dollar",
    "SSP": "South Sudanese Pound",
    "STN": "São Tomé and Príncipe Dobra",
    "SYP": "Syrian Pound",
    "SZL": "Swazi Lilangeni",
    "THB": "Thai Baht",
    "TJS": "Tajikistani Somoni",
    "TMT": "Turkmenistani Manat",
    "TND": "Tunisian Dinar",
    "TOP": "Tongan Paʻanga",
    "TRY": "Turkish Lira",
    "TTD": "Trinidad and Tobago Dollar",
    "TWD": "New Taiwan Dollar",
    "TZS": "Tanzanian Shilling",
    "UAH": "Ukrainian Hryvnia",
    "UGX": "Ugandan Shilling",
    "USD": "United States Dollar",
    "UYU": "Uruguayan Peso",
    "UZS": "Uzbekistan Som",
    "VES": "Venezuelan Bolívar",
    "VND": "Vietnamese Dong",
    "VUV": "Vanuatu Vatu",
    "WST": "Samoan Tala",
    "XAF": "CFA Franc BEAC",
    "XCD": "East Caribbean Dollar",
    "XOF": "CFA Franc BCEAO",
    "XPF": "CFP Franc",
    "YER": "Yemeni Rial",
    "ZAR": "South African Rand",
    "ZMW": "Zambian Kwacha",
    "ZWL": "Zimbabwean Dollar"
};

export default function CurrencyConverterPopup() {
  const [showPopup, setShowPopup] = useState(false);
  const [fromCurrency, setFromCurrency] = useState("GBP");
  const [toCurrency, setToCurrency] = useState("NGN");
  const [amount, setAmount] = useState("");
  const [convertedAmount, setConvertedAmount] = useState(null);
  const [exchangeRate, setExchangeRate] = useState(null);
  const [loading, setLoading] = useState(false);
  const [fetchingData, setFetchingData] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const handleOpenPopup = () => {
    setShowPopup(true);
    setError("");
    setSuccess("");
    setConvertedAmount(null);
    setExchangeRate(null);
  };

  const handleConvert = async (e) => {
    e.preventDefault();
    
    if (!amount || isNaN(amount)) {
      setError("Please enter a valid amount");
      return;
    }

    setLoading(true);
    setError("");
    setSuccess("");

    try {
      const response = await API.get(
        `/CurrencyConverter/convert?FromCurrency=${fromCurrency}&ToCurrency=${toCurrency}&Amount=${amount}`
      );
      
      let result;
      if (typeof response.data === 'number') {
        result = response.data;
      } else if (response.data?.convertedAmount) {
        result = response.data.convertedAmount;
      } else if (response.data?.result) {
        result = response.data.result;
      } else if (response.data?.amount) {
        result = response.data.amount;
      } else {
        throw new Error("Unexpected response format from server");
      }

      if (typeof result !== 'number' || isNaN(result)) {
        throw new Error("Invalid conversion result");
      }

      setConvertedAmount(result);
      
      if (amount && result) {
        const rate = parseFloat(result) / parseFloat(amount);
        setExchangeRate(rate.toFixed(2));
      }

      setSuccess("Conversion successful!");
    } catch (err) {
      console.error("Conversion error:", err);
      setError(err.response?.data?.message || err.message || "Conversion failed. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const handleSwap = () => {
    setFromCurrency(toCurrency);
    setToCurrency(fromCurrency);
    setConvertedAmount(null);
    setExchangeRate(null);
  };

    const handleAmountChange = (e) => {
    // Remove any non-numeric characters except decimal point
    let value = e.target.value.replace(/[^0-9.]/g, '');
    
    // Ensure only one decimal point
    const decimalCount = value.split('.').length - 1;
    if (decimalCount > 1) {
      value = value.substring(0, value.lastIndexOf('.'));
    }
    
    // Limit to 2 decimal places
    if (value.includes('.')) {
      const parts = value.split('.');
      if (parts[1].length > 2) {
        value = `${parts[0]}.${parts[1].substring(0, 2)}`;
      }
    }
    
    setAmount(value);
  };

  const handleKeyPress = (e) => {
    // Prevent typing letters or multiple decimal points
    if (!/[0-9.]/.test(e.key) || 
        (e.key === '.' && amount.includes('.'))) {
      e.preventDefault();
    }
  };

  const handlePaste = (e) => {
    // Get pasted data
    const pasteData = e.clipboardData.getData('text/plain');
    
    // Validate pasted content
    if (!/^[0-9.]*$/.test(pasteData)) {
      e.preventDefault();
    }
  };

  const formatAmount = (value) => {
    if (value === null || value === undefined) return "0.00";
    if (typeof value === 'number') {
      return value.toLocaleString(undefined, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      });
    }
    return parseFloat(value).toLocaleString(undefined, {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
  };

  return (
  <>
    <motion.button
      className={styles.fab}
      onClick={handleOpenPopup}
      whileHover={{ scale: 1.05 }}
      whileTap={{ scale: 0.95 }}
    >
      <SwapHorizIcon className={styles.fabIcon} />
      <span>Currency Converter</span>
    </motion.button>

      {showPopup && (
        <div className={styles.popupOverlay}>
          <div className={styles.popupContent}>
            <button
              className={styles.closeButton}
              onClick={() => {
                setShowPopup(false);
              }}
            >
              ✕
            </button>

            <h2>Currency Converter</h2>
            <p>Convert between different currencies</p>

            <form onSubmit={handleConvert} className={styles.formContainer}>
              <div className={styles.currencyRow}>
                <div className={styles.currencyGroup}>
                  <label className={styles.currencyLabel}>From</label>
                  <select
                    className={styles.currencySelect}
                    value={fromCurrency}
                    onChange={(e) => setFromCurrency(e.target.value)}
                  >
                    {currencyCodes.map((code) => (
                      <option key={`from-${code}`} value={code}>
                        {code} - {currencyNames[code] || code}
                      </option>
                    ))}
                  </select>
                </div>

                <button 
                  type="button" 
                  className={styles.swapButton}
                  onClick={handleSwap}
                  aria-label="Swap currencies"
                >
                  <SwapHorizIcon />
                </button>

                <div className={styles.currencyGroup}>
                  <label className={styles.currencyLabel}>To</label>
                  <select
                    className={styles.currencySelect}
                    value={toCurrency}
                    onChange={(e) => setToCurrency(e.target.value)}
                  >
                    {currencyCodes.map((code) => (
                      <option key={`to-${code}`} value={code}>
                        {code} - {currencyNames[code] || code}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <div className={styles.amountGroup}>
                <label className={styles.currencyLabel}>Amount</label>
                <div className={styles.amountInputWrapper}>
                  <span className={styles.currencySymbol}>
                    {currencySymbols[fromCurrency] || fromCurrency}
                  </span>
                  <input
                    className={styles.amountInput}
                    type="number"
                    value={amount}
                    onChange={(e) => setAmount(e.target.value)}
                    placeholder="0.00"
                    min="0.01"
                    step="0.01"
                    required
                  />
                </div>
              </div>

              {convertedAmount !== null && (
                <div className={styles.resultContainer}>
                  <div className={styles.resultAmount}>
                    {formatAmount(amount)} {fromCurrency} ={" "}
                    <strong>{formatAmount(convertedAmount)}</strong> {toCurrency}
                  </div>
                  {exchangeRate && (
                    <div className={styles.exchangeRate}>
                      <strong>Exchange rate</strong>: 1 {fromCurrency} = {formatAmount(exchangeRate)} {toCurrency}
                    </div>
                  )}
                </div>
              )}

              {error && <div className={styles.error}>{error}</div>}
              {success && <div className={styles.success}>{success}</div>}

              <button
                type="submit"
                className={styles.submitButton}
                disabled={loading}
              >
                {loading ? "Converting..." : "Convert"}
              </button>
            </form>
          </div>
        </div>
      )}
    </>
  );
}