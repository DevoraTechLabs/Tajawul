import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:intl/intl.dart';
import 'package:dio/dio.dart';
import 'package:path/path.dart' as p;
import 'package:permission_handler/permission_handler.dart';
import 'package:device_info_plus/device_info_plus.dart';
import 'dart:io';

class CompleteProfilePage extends StatefulWidget {
  @override
  _CompleteProfilePageState createState() => _CompleteProfilePageState();
}

class _CompleteProfilePageState extends State<CompleteProfilePage> {
  final _formKey = GlobalKey<FormState>();
  final Dio _dio = Dio();
  final DeviceInfoPlugin _deviceInfo = DeviceInfoPlugin();

  String? _gender = "Male";
  String? _firstName;
  String? _lastName;
  String? _username;
  String? _bio;
  String? _phoneNumber;
  String? _maritalStatus;
  String? _birthDate;
  String? _country;
  String? _city;
  String? _nationality;
  List<String> _spokenLanguages = [];
  PlatformFile? _selectedFile;
  String? _profileImageUrl;
  bool _isLoading = false;

  final List<String> _maritalStatusOptions = [
    "Single",
    "Married",
    "Divorced",
    "Widowed"
  ];
  final List<String> _countryOptions = ["USA", "Canada", "Egypt", "UK", "UAE"];
  final List<String> _cityOptions = [
    "New York",
    "Toronto",
    "Cairo",
    "London",
    "Dubai"
  ];
  final List<String> _nationalityOptions = [
    "American",
    "Canadian",
    "Egyptian",
    "British",
    "Emirati"
  ];
  final List<String> _languageOptions = [
    "English",
    "Arabic",
    "Spanish",
    "French",
    "German"
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Complete Your Profile"),
        backgroundColor: Colors.brown,
        foregroundColor: Colors.white,
      ),
      body: Stack(
        children: [
          SingleChildScrollView(
            padding: EdgeInsets.all(16),
            child: Form(
              key: _formKey,
              child: Column(
                children: [
                  _buildTitleSection(),
                  SizedBox(height: 20),
                  _buildProfileImageSection(),
                  SizedBox(height: 20),
                  _buildPersonalInfoSection(),
                  _buildLocationInfoSection(),
                  _buildGenderRadio(),
                  _buildMultiSelect("Spoken Languages", _languageOptions),
                  SizedBox(height: 30),
                  _buildSubmitButton(),
                ],
              ),
            ),
          ),
          if (_isLoading)
            Container(
              color: Colors.black.withOpacity(0.5),
              child: Center(
                child: CircularProgressIndicator(
                  valueColor: AlwaysStoppedAnimation<Color>(Colors.brown),
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildTitleSection() {
    return Column(
      children: [
        Text(
          "Complete Your Profile",
          style: TextStyle(
              fontSize: 24, fontWeight: FontWeight.bold, color: Colors.brown),
        ),
        SizedBox(height: 8),
        Text(
          "Please provide your information to complete your profile",
          style: TextStyle(color: Colors.grey),
        ),
      ],
    );
  }

  Widget _buildProfileImageSection() {
    return Column(
      children: [
        Stack(
          children: [
            CircleAvatar(
              radius: 50,
              backgroundImage: _getImageProvider(),
              child: _selectedFile == null && _profileImageUrl == null
                  ? Icon(Icons.person, size: 50)
                  : null,
            ),
            if (_isLoading)
              Positioned.fill(
                child: CircularProgressIndicator(
                  valueColor: AlwaysStoppedAnimation<Color>(Colors.brown),
                ),
              ),
          ],
        ),
        SizedBox(height: 10),
        ElevatedButton(
          onPressed: _pickProfileImage,
          child: Text("Upload Profile Picture"),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.brown,
            foregroundColor: Colors.white,
          ),
        ),
      ],
    );
  }

  Widget _buildPersonalInfoSection() {
    return Column(
      children: [
        Row(
          children: [
            Expanded(
                child: _buildTextField("First Name",
                    onSaved: (v) => _firstName = v,
                    validator: _requiredValidator)),
            SizedBox(width: 10),
            Expanded(
                child: _buildTextField("Last Name",
                    onSaved: (v) => _lastName = v,
                    validator: _requiredValidator)),
          ],
        ),
        _buildTextField("Username",
            onSaved: (v) => _username = v, validator: _requiredValidator),
        _buildTextField("Bio", maxLines: 3, onSaved: (v) => _bio = v),
        _buildTextField("Phone Number",
            onSaved: (v) => _phoneNumber = v,
            validator: _requiredValidator,
            keyboardType: TextInputType.phone),
        _buildDatePicker("Birth Date"),
        _buildDropdown("Marital Status", _maritalStatusOptions,
            value: _maritalStatus,
            onChanged: (v) => setState(() => _maritalStatus = v)),
      ],
    );
  }

  Widget _buildLocationInfoSection() {
    return Column(
      children: [
        _buildDropdown("Nationality", _nationalityOptions,
            value: _nationality,
            onChanged: (v) => setState(() => _nationality = v)),
        Row(
          children: [
            Expanded(
                child: _buildDropdown("Country", _countryOptions,
                    value: _country,
                    onChanged: (v) => setState(() => _country = v))),
            SizedBox(width: 10),
            Expanded(
                child: _buildDropdown("City", _cityOptions,
                    value: _city, onChanged: (v) => setState(() => _city = v))),
          ],
        ),
      ],
    );
  }

  Widget _buildTextField(
    String label, {
    int maxLines = 1,
    void Function(String?)? onSaved,
    String? Function(String?)? validator,
    TextInputType? keyboardType,
  }) {
    return Padding(
      padding: EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        decoration: InputDecoration(
          labelText: label,
          border: OutlineInputBorder(),
          contentPadding: EdgeInsets.symmetric(horizontal: 12, vertical: 12),
        ),
        maxLines: maxLines,
        validator: validator,
        onSaved: onSaved,
        keyboardType: keyboardType,
      ),
    );
  }

  String? _requiredValidator(String? value) {
    return value == null || value.isEmpty ? 'Required' : null;
  }

  Widget _buildDatePicker(String label) {
    return Padding(
      padding: EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        readOnly: true,
        decoration: InputDecoration(
          labelText: label,
          border: OutlineInputBorder(),
          suffixIcon: Icon(Icons.calendar_today),
          contentPadding: EdgeInsets.symmetric(horizontal: 12, vertical: 12),
        ),
        controller: TextEditingController(
          text: _birthDate ?? '',
        ),
        validator: _requiredValidator,
        onTap: () async {
          FocusScope.of(context).requestFocus(FocusNode());
          DateTime? picked = await showDatePicker(
            context: context,
            initialDate: _birthDate != null
                ? DateFormat('dd-MM-yyyy').parse(_birthDate!)
                : DateTime.now().subtract(Duration(days: 365 * 18)),
            firstDate: DateTime(1900),
            lastDate: DateTime.now(),
            builder: (context, child) {
              return Theme(
                data: Theme.of(context).copyWith(
                  colorScheme: ColorScheme.light(
                    primary: Colors.brown,
                    onPrimary: Colors.white,
                    onSurface: Colors.black,
                  ),
                  textButtonTheme: TextButtonThemeData(
                    style: TextButton.styleFrom(
                      foregroundColor: Colors.brown,
                    ),
                  ),
                ),
                child: child!,
              );
            },
          );
          if (picked != null) {
            setState(
                () => _birthDate = DateFormat('dd-MM-yyyy').format(picked));
          }
        },
      ),
    );
  }

  Widget _buildDropdown(
    String label,
    List<String> options, {
    String? value,
    void Function(String?)? onChanged,
  }) {
    return Padding(
      padding: EdgeInsets.symmetric(vertical: 8),
      child: DropdownButtonFormField<String>(
        decoration: InputDecoration(
          labelText: label,
          border: OutlineInputBorder(),
          contentPadding: EdgeInsets.symmetric(horizontal: 12, vertical: 12),
        ),
        value: value,
        items: options
            .map((e) => DropdownMenuItem(value: e, child: Text(e)))
            .toList(),
        onChanged: onChanged,
        validator: _requiredValidator,
      ),
    );
  }

  Widget _buildGenderRadio() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: EdgeInsets.symmetric(vertical: 8),
          child: Text("Gender", style: TextStyle(fontSize: 16)),
        ),
        Row(
          children: ["Male", "Female"].map((gender) {
            return Expanded(
              child: RadioListTile<String>(
                value: gender,
                groupValue: _gender,
                title: Text(gender),
                onChanged: (value) => setState(() => _gender = value),
                contentPadding: EdgeInsets.zero,
              ),
            );
          }).toList(),
        )
      ],
    );
  }

  Widget _buildMultiSelect(String label, List<String> options) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: EdgeInsets.symmetric(vertical: 8),
          child: Text(label, style: TextStyle(fontSize: 16)),
        ),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: options.map((language) {
            return FilterChip(
              label: Text(language),
              selected: _spokenLanguages.contains(language),
              onSelected: (selected) {
                setState(() {
                  selected
                      ? _spokenLanguages.add(language)
                      : _spokenLanguages.remove(language);
                });
              },
              selectedColor: Colors.brown.withOpacity(0.2),
              checkmarkColor: Colors.brown,
              labelStyle: TextStyle(
                color: _spokenLanguages.contains(language)
                    ? Colors.brown
                    : Colors.black,
              ),
            );
          }).toList(),
        ),
      ],
    );
  }

  Widget _buildSubmitButton() {
    return SizedBox(
      width: double.infinity,
      child: ElevatedButton(
        onPressed: _submitProfile,
        child: Text("Save Profile"),
        style: ElevatedButton.styleFrom(
          backgroundColor: Colors.brown,
          foregroundColor: Colors.white,
          padding: EdgeInsets.symmetric(vertical: 16),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(8),
          ),
        ),
      ),
    );
  }

  ImageProvider? _getImageProvider() {
    if (_selectedFile != null && _selectedFile!.path != null) {
      return FileImage(File(_selectedFile!.path!));
    } else if (_profileImageUrl != null) {
      return NetworkImage(_profileImageUrl!);
    }
    return null;
  }

  Future<void> _pickProfileImage() async {
    try {
      setState(() => _isLoading = true);

      final androidInfo = await _deviceInfo.androidInfo;
      PermissionStatus status;

      if (Platform.isAndroid && androidInfo.version.sdkInt >= 33) {
        status = await Permission.photos.request();
      } else {
        status = await Permission.storage.request();
      }

      if (!status.isGranted) {
        bool? shouldOpenSettings = await showDialog<bool>(
          context: context,
          builder: (context) => AlertDialog(
            title: Text("Permission Required"),
            content: Text("Allow storage access to select profile pictures."),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(context, false),
                child: Text("Cancel"),
              ),
              TextButton(
                onPressed: () => Navigator.pop(context, true),
                child: Text("Open Settings"),
              ),
            ],
          ),
        );

        if (shouldOpenSettings == true) {
          await openAppSettings();
        }
        return;
      }

      FilePickerResult? result = await FilePicker.platform.pickFiles(
        type: FileType.image,
        allowMultiple: false,
      );

      if (result != null && result.files.isNotEmpty) {
        final file = result.files.first;
        if (file.path == null) return;

        if (!await File(file.path!).exists()) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('File not found or inaccessible.')),
          );
          return;
        }

        setState(() => _selectedFile = file);
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error: ${e.toString()}')),
      );
    } finally {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _submitProfile() async {
    if (!_formKey.currentState!.validate()) return;
    _formKey.currentState!.save();

    if (_selectedFile == null || _selectedFile!.path == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Please select a profile image')),
      );
      return;
    }

    setState(() => _isLoading = true);

    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString('auth_token') ?? '';

      final file = File(_selectedFile!.path!);
      if (!await file.exists()) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Selected file no longer exists')),
        );
        return;
      }

      // Upload image
      String fileName = p.basename(file.path);
      FormData formData = FormData.fromMap({
        "ProfileImage": await MultipartFile.fromFile(
          file.path,
          filename: fileName,
        ),
      });

      var imageResponse = await _dio.put(
        'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/User/profile/image',
        data: formData,
        options: Options(
          headers: {
            'accept': '*/*',
            'Authorization': 'Bearer $token',
            'Content-Type': 'multipart/form-data',
          },
        ),
      );

      if (imageResponse.statusCode == 200) {
        _profileImageUrl = imageResponse.data['url'];
      } else {
        throw Exception('Failed to upload image');
      }

      // Convert date format from dd-MM-yyyy to yyyy-MM-dd
      String formattedBirthDate = '';
      if (_birthDate != null && _birthDate!.isNotEmpty) {
        try {
          DateTime parsedDate = DateFormat('dd-MM-yyyy').parse(_birthDate!);
          formattedBirthDate = DateFormat('yyyy-MM-dd').format(parsedDate);
        } catch (e) {
          throw Exception('Invalid date format');
        }
      }

      // Save profile info
      final profileUrl = Uri.parse(
          'https://tajawul-caddcdduayewd2bv.uaenorth-01.azurewebsites.net/api/User/info');

      final profileBody = {
        "username": _username,
        "firstName": _firstName,
        "lastName": _lastName,
        "bio": _bio,
        "phoneNumber": _phoneNumber,
        "gender": _gender,
        "maritalStatus": _maritalStatus,
        "birthDate": formattedBirthDate, // Now in yyyy-MM-dd format
        "country": _country,
        "city": _city,
        "nationality": _nationality,
        "spokenLanguages": _spokenLanguages,
        "socialMediaLinks": []
      };

      final profileResponse = await http.post(
        profileUrl,
        headers: {
          'accept': '*/*',
          'Authorization': 'Bearer $token',
          'Content-Type': 'application/json',
        },
        body: jsonEncode(profileBody),
      );

      if (profileResponse.statusCode == 200) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Profile saved successfully!')),
        );
        Navigator.pushReplacementNamed(context, '/profilecom');
      } else {
        throw Exception('Failed to save profile: ${profileResponse.body}');
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error: ${e.toString()}')),
      );
    } finally {
      setState(() => _isLoading = false);
    }
  }
}
