import pandas as pd

# Load datasets
routes_data = pd.read_csv(r'C:\Users\abhinraw\Documents\AbhinavBackup\Code\DataSets\routes.csv')
airports_geoLoc_data = pd.read_csv(r'C:\Users\abhinraw\Documents\AbhinavBackup\Code\DataSets\airports-geoLoc.csv')
airlines_data = pd.read_csv(r'C:\Users\abhinraw\Documents\AbhinavBackup\Code\DataSets\airlines.csv')

# User input for source and destination locations
source_location = input("Enter the source airport location (name or IATA code): ").upper()
destination_location = input("Enter the destination airport location (name or IATA code): ").upper()

# Find airport IDs for source and destination locations
source_airports = airports_geoLoc_data[(airports_geoLoc_data['Airport Name'].str.upper() == source_location) | (airports_geoLoc_data['IATA'] == source_location)]
destination_airports = airports_geoLoc_data[(airports_geoLoc_data['Airport Name'].str.upper() == destination_location) | (airports_geoLoc_data['IATA'] == destination_location)]

# Check if source and destination locations exist
if len(source_airports) == 0:
    print("Source airport not found.")
    exit()
if len(destination_airports) == 0:
    print("Destination airport not found.")
    exit()

# Display source and destination locations to the user
print("Source Airport:", source_airports[['Airport Name', 'City Name', 'Country Name', 'IATA']])
print("Destination Airport:", destination_airports[['Airport Name', 'City Name', 'Country Name', 'IATA']])

# Retrieve airport IDs for source and destination locations
source_airport_id = source_airports['ID'].values
destination_airport_id = destination_airports['ID'].values

# Find routes based on source and destination airport IDs
relevant_routes = routes_data[(routes_data[' source airport id'].isin(source_airport_id)) & (routes_data[' destination airport id'].isin(destination_airport_id))]

# Print relevant routes
print("Relevant Routes:")
print(relevant_routes)

# You can further process the relevant routes data or fetch additional information from airlines_data if needed.
