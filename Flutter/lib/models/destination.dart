// models/destination.dart
class Destination {
  final String destinationId;
  final String name;
  final String description;
  final String coverImage;
  final String type;
  final String country;
  final String city;
  final String priceRange;
  final bool isVerified;
  final bool isOpen24Hours;
  final String? openTime;
  final String? closeTime;
  final double averageRating;
  final int visitorsCount;
  final List<Location> locations;
  final List<String> images;
  final List<ContactInfo> contactInfo;
  final List<SocialMediaLink> socialMediaLinks;
  final String? establishedAt;
  final String creationDate;
  final String lastEditDate;

  Destination({
    required this.destinationId,
    required this.name,
    required this.description,
    required this.coverImage,
    required this.type,
    required this.country,
    required this.city,
    required this.priceRange,
    required this.isVerified,
    required this.isOpen24Hours,
    this.openTime,
    this.closeTime,
    required this.averageRating,
    required this.visitorsCount,
    required this.locations,
    required this.images,
    required this.contactInfo,
    required this.socialMediaLinks,
    this.establishedAt,
    required this.creationDate,
    required this.lastEditDate,
  });

  factory Destination.fromJson(Map<String, dynamic> json) {
    return Destination(
      destinationId: json['destinationId'] ?? '',
      name: json['name'] ?? '',
      description: json['description'] ?? '',
      coverImage: json['coverImage'] ?? '',
      type: json['type'] ?? '',
      country: json['country'] ?? '',
      city: json['city'] ?? '',
      priceRange: json['priceRange'] ?? '',
      isVerified: json['isVerified'] ?? false,
      isOpen24Hours: json['isOpen24Hours'] ?? false,
      openTime: json['openTime'],
      closeTime: json['closeTime'],
      averageRating: (json['averageRating'] ?? 0).toDouble(),
      visitorsCount: json['visitorsCount'] ?? 0,
      locations: (json['locations'] as List?)
              ?.map((loc) => Location.fromJson(loc))
              .toList() ??
          [],
      images: (json['images'] as List?)?.cast<String>() ?? [],
      contactInfo: (json['contactInfo'] as List?)
              ?.map((ci) => ContactInfo.fromJson(ci))
              .toList() ??
          [],
      socialMediaLinks: (json['socialMediaLinks'] as List?)
              ?.map((sml) => SocialMediaLink.fromJson(sml))
              .toList() ??
          [],
      establishedAt: json['establishedAt'],
      creationDate: json['creationDate'] ?? '',
      lastEditDate: json['lastEditDate'] ?? '',
    );
  }
}

class Location {
  final double longitude;
  final double latitude;
  final String address;

  Location({
    required this.longitude,
    required this.latitude,
    required this.address,
  });

  factory Location.fromJson(Map<String, dynamic> json) {
    return Location(
      longitude: (json['longitude'] ?? 0).toDouble(),
      latitude: (json['latitude'] ?? 0).toDouble(),
      address: json['address'] ?? '',
    );
  }
}

class ContactInfo {
  final String type;
  final String value;

  ContactInfo({
    required this.type,
    required this.value,
  });

  factory ContactInfo.fromJson(Map<String, dynamic> json) {
    return ContactInfo(
      type: json['type']?.toString() ?? '',
      value: json['value'] ?? '',
    );
  }
}

class SocialMediaLink {
  final String platform;
  final String url;

  SocialMediaLink({
    required this.platform,
    required this.url,
  });

  factory SocialMediaLink.fromJson(Map<String, dynamic> json) {
    return SocialMediaLink(
      platform: json['platform'] ?? '',
      url: json['url'] ?? '',
    );
  }
}

class DestinationResponse {
  final int count;
  final List<Destination> destinations;

  DestinationResponse({
    required this.count,
    required this.destinations,
  });

  factory DestinationResponse.fromJson(Map<String, dynamic> json) {
    return DestinationResponse(
      count: json['count'] ?? 0,
      destinations: (json['destinations'] as List?)
              ?.map((dest) => Destination.fromJson(dest))
              .toList() ??
          [],
    );
  }
}
