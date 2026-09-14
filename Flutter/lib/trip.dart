// trip.dart
enum PriceLevel { low, midRange, luxury }

class Trip {
  final String tripId;
  final String title;
  final String duration;
  final String countryType;
  final PriceLevel priceLevel;
  final String imageUrl;
  final String description;
  final String status;
  final String visibility;
  final int destinationCount;

  Trip({
    required this.tripId,
    required this.title,
    required this.duration,
    required this.countryType,
    required this.priceLevel,
    required this.imageUrl,
    required this.description,
    required this.status,
    required this.visibility,
    required this.destinationCount,
  });

  factory Trip.fromJson(Map<String, dynamic> json) {
    return Trip(
      tripId: json['tripId'] ?? '',
      title: json['title'] ?? '',
      duration: _mapDuration(json['tripDuration']),
      countryType: json['sameCountry'] ? 'Same Country' : 'Different Countries',
      priceLevel: _mapPriceLevel(json['priceRange']),
      imageUrl: json['coverImage'] ?? '',
      description: json['description'] ?? '',
      status: json['status'] ?? '',
      visibility: json['visibility'] ?? '',
      destinationCount: json['destinationCount'] ?? 0,
    );
  }

  static String _mapDuration(String? duration) {
    switch (duration) {
      case 'Short':
        return '1-3 days (Short)';
      case 'Mid':
        return '4-7 days (Mid)';
      case 'Long':
        return '8+ days (Long)';
      default:
        return duration ?? 'Unknown duration';
    }
  }

  static PriceLevel _mapPriceLevel(String? priceRange) {
    switch (priceRange) {
      case 'Low':
        return PriceLevel.low;
      case 'Mid':
        return PriceLevel.midRange;
      case 'Luxury':
        return PriceLevel.luxury;
      default:
        return PriceLevel.low;
    }
  }
}

class ApiDestination {
  final String destinationId;
  final int day;
  final String name;
  final String type;
  final String coverImage;
  final List<String> images;
  final String city;
  final String country;
  final List<Location> locations;
  final bool isOpen24Hours;
  final String openTime;
  final String closeTime;
  final String priceRange;
  final double averageRating;
  final int reviewsCount;

  ApiDestination({
    required this.destinationId,
    required this.day,
    required this.name,
    required this.type,
    required this.coverImage,
    required this.images,
    required this.city,
    required this.country,
    required this.locations,
    required this.isOpen24Hours,
    required this.openTime,
    required this.closeTime,
    required this.priceRange,
    required this.averageRating,
    required this.reviewsCount,
  });

  factory ApiDestination.fromJson(Map<String, dynamic> json) {
    return ApiDestination(
      destinationId: json['destinationId'] ?? '',
      day: json['day'] ?? 0,
      name: json['name'] ?? '',
      type: json['type'] ?? '',
      coverImage: json['coverImage'] ?? '',
      images: List<String>.from(json['images'] ?? []),
      city: json['city'] ?? '',
      country: json['country'] ?? '',
      locations: List<Location>.from(
          (json['locations'] ?? []).map((x) => Location.fromJson(x))),
      isOpen24Hours: json['isOpen24Hours'] ?? false,
      openTime: json['openTime'] ?? '',
      closeTime: json['closeTime'] ?? '',
      priceRange: json['priceRange'] ?? '',
      averageRating: (json['averageRating'] ?? 0).toDouble(),
      reviewsCount: json['reviewsCount'] ?? 0,
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
