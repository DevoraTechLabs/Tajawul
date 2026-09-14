import ReactCountryFlag from "react-country-flag";

const countryFlags = {
  Algeria: (props) => <ReactCountryFlag countryCode="DZ" svg {...props} />,
  Bahrain: (props) => <ReactCountryFlag countryCode="BH" svg {...props} />,
  Comoros: (props) => <ReactCountryFlag countryCode="KM" svg {...props} />,
  Djibouti: (props) => <ReactCountryFlag countryCode="DJ" svg {...props} />,
  Egypt: (props) => <ReactCountryFlag countryCode="EG" svg {...props} />,
  Iraq: (props) => <ReactCountryFlag countryCode="IQ" svg {...props} />,
  Jordan: (props) => <ReactCountryFlag countryCode="JO" svg {...props} />,
  Kuwait: (props) => <ReactCountryFlag countryCode="KW" svg {...props} />,
  Lebanon: (props) => <ReactCountryFlag countryCode="LB" svg {...props} />,
  Libya: (props) => <ReactCountryFlag countryCode="LY" svg {...props} />,
  Mauritania: (props) => <ReactCountryFlag countryCode="MR" svg {...props} />,
  Morocco: (props) => <ReactCountryFlag countryCode="MA" svg {...props} />,
  Oman: (props) => <ReactCountryFlag countryCode="OM" svg {...props} />,
  Palestine: (props) => <ReactCountryFlag countryCode="PS" svg {...props} />,
  Qatar: (props) => <ReactCountryFlag countryCode="QA" svg {...props} />,
  "Saudi Arabia": (props) => (
    <ReactCountryFlag countryCode="SA" svg {...props} />
  ),
  Somalia: (props) => <ReactCountryFlag countryCode="SO" svg {...props} />,
  "South Sudan": (props) => (
    <ReactCountryFlag countryCode="SS" svg {...props} />
  ),
  Sudan: (props) => <ReactCountryFlag countryCode="SD" svg {...props} />,
  Syria: (props) => <ReactCountryFlag countryCode="SY" svg {...props} />,
  Tunisia: (props) => <ReactCountryFlag countryCode="TN" svg {...props} />,
  "United Arab Emirates": (props) => (
    <ReactCountryFlag countryCode="AE" svg {...props} />
  ),
  Yemen: (props) => <ReactCountryFlag countryCode="YE" svg {...props} />,
};

export default countryFlags;
