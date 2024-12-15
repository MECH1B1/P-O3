import pandas as pd
import matplotlib.pyplot as plt
import csv


def transform(data):
    return data / 500 - 0.3


def shift_sensor_data(input_csv, output_csv, delay_, snapshot_interval=0.1):
    """
    Shifts sensor data in a CSV file based on the specified delay.

    Parameters:
    - input_csv (str): Path to the input CSV file.
    - output_csv (str): Path to the output CSV file.
    - delay_ (float): The delay (in seconds) for shifting sensor data. Can be positive or negative.
    - snapshot_interval (float): The time interval between snapshots (default is 0.1 seconds).
    """
    # Read the input CSV
    data_df = pd.read_csv(input_csv)

    # Calculate the shift amount (in rows) for the sensor data
    shift_amount = int(round(delay_ / snapshot_interval))

    # Prepare shifted sensor data
    shifted_data = data_df.copy()

    # If the delay is positive, shift sensor columns forward; if negative, shift backward
    if shift_amount > 0:
        shifted_data.iloc[shift_amount:, 7:] = data_df.iloc[:-shift_amount, 7:].values
        shifted_data.iloc[:shift_amount, 7:] = 0  # Fill leading rows with zeros
    elif shift_amount < 0:
        shifted_data.iloc[:shift_amount, 7:] = data_df.iloc[-shift_amount:, 7:].values
        shifted_data.iloc[shift_amount:, 7:] = 0  # Fill trailing rows with zeros

    # Write the shifted data to the output CSV file
    shifted_data.to_csv(output_csv, index=False)
    print(f"CSV file successfully processed and saved as {output_csv}")
    print(f"Shift amount (rows): {shift_amount}")


# Input file path
input_file = "HandAnimatorParametersWithSensors.csv"
output_file = "converted.csv"
delay = -1.2

# Shift sensor data with the specified delay
shift_sensor_data(input_file, output_file, delay)

# Load the shifted CSV file into a DataFrame
data = pd.read_csv(output_file)

# Specify the parameter to plot (IndexBlend1 is the first parameter for the index finger)
parameter_name = "IndexBlend1"

# Define lower and upper bounds for time
time_lower_bound = 570
time_upper_bound = 615

# Filter data based on time bounds
filtered_data = data[(data['Time'] >= time_lower_bound) & (data['Time'] <= time_upper_bound)]

# Check if the parameter exists in the data
if parameter_name not in data.columns:
    print(f"Parameter '{parameter_name}' not found in the data.")
else:
    # Plot the parameter, Sensor 1, and Sensor 2 values against time
    plt.figure(figsize=(10, 6))

    # Plot the first index finger parameter
    plt.plot(filtered_data['Time'], filtered_data[parameter_name], label="Parameter", color='blue', linewidth=2)

    # Plot Sensor 2
    plt.plot(filtered_data['Time'], transform(filtered_data.iloc[:, 8]), label='Buigsensor', color='red', linestyle=':')

    # Configure the plot
    plt.title(f"Een handparameter en buigsensor van de wijsvinger geplot over tijd")
    plt.xlabel("Tijd")
    plt.ylabel("Waarden")
    plt.legend()
    plt.grid(True)
    plt.tight_layout()

    # Save the plot as a high-quality PNG
    output_image_path = f"na_tijdshift.png"
    plt.savefig(output_image_path, dpi=300, format='png')  # Set dpi for high-quality output
    print(f"Filtered plot saved as {output_image_path}")

    # Display the plot
    plt.show()
