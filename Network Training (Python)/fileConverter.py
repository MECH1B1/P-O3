import pandas as pd
import matplotlib
matplotlib.use("TkAgg")
import matplotlib.pyplot as plt
import csv

def transform(data):
    return data / 200 - 0.8


def shift_sensor_data(input_csv, output_csv, delay_, remove_value, snapshot_interval=0.1):
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

    # Remove rows where any sensor column contains the value 1023
    sensor_columns = data_df.columns[7:]  # Assuming sensor columns start at index 7
    data_df = data_df[~(data_df[sensor_columns] >= remove_value).any(axis=1)]

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
    print(f"Rows removed with a value above {remove_value}: {len(data_df) - len(shifted_data)}")

def merge_csv_files(input_csv1, input_csv2, output_csv="merged.csv"):
    """
    Merges two CSV files with the same format into a single file.

    Parameters:
    - input_csv1 (str): Path to the first input CSV file.
    - input_csv2 (str): Path to the second input CSV file.
    - output_csv (str): Path to save the merged CSV file (default is 'merged.csv').
    """
    try:
        # Load the two CSV files into DataFrames
        df1 = pd.read_csv(input_csv1)
        df2 = pd.read_csv(input_csv2)

        print(f"Rows in {input_csv1}: {len(df1)}")
        print(f"Rows in {input_csv2}: {len(df2)}")

        # Append the second DataFrame to the first
        merged_df = pd.concat([df1, df2], ignore_index=True)

        # Save the merged DataFrame to a new CSV file
        merged_df.to_csv(output_csv, index=False)

        print(f"Merged file saved as: {output_csv}")
        print(f"Total rows in merged file: {len(merged_df)}")

    except Exception as e:
        print(f"An error occurred: {e}")

def plot_data(data_file, parameter_name, sensor_number, time_lower_bound, time_upper_bound):
    # Load the shifted CSV file into a DataFrame
    data = pd.read_csv(data_file)

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
        plt.plot(filtered_data['Time'], transform(filtered_data.iloc[:, 7+sensor_number]), label='Sensor', color='red',
                 linestyle=':')

        # Configure the plot
        plt.title(f"Een handparameter en sensor van de wijsvinger geplot over tijd")
        plt.xlabel("Tijd")
        plt.ylabel("Waarden")
        plt.legend()
        plt.grid(True)
        plt.tight_layout()

        # Display the plot
        plt.show()


if __name__ == "__main__":

    # Input file path
    input_file = "HandAnimatorParametersWithSensors.csv"
    output_file = "converted.csv"

    # Shift sensor data with the specified delay
    shift_sensor_data(input_file, output_file,-0.1 ,900)
    plot_data(output_file, "ThumbBlend1", 3, 0, 1000)

    """
    # File names can be adjusted here
    input_file1 = "converted1.csv"  # Change to the name of your first CSV file
    input_file2 = "converted2.csv"  # Change to the name of your second CSV file
    output_file = "merged.csv"  # Name of the output file

    # Merge the CSV files
    merge_csv_files(input_file1, input_file2, output_file)
    """