import tensorflow as tf
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler
import joblib
import json
import tf2onnx


def create_network(X_train, y_train, X_test, y_test, scaler_X, scaler_y, sensor_columns, model_name):
    # Define the neural network using the Functional API
    inputs = tf.keras.Input(shape=(X_train.shape[1],), name="input_layer")
    x = tf.keras.layers.Dense(8, activation='relu', name="1_dense_8")(inputs)
    x = tf.keras.layers.Dense(8, activation='relu', name="2_dense_8")(x)
    x = tf.keras.layers.Dense(8, activation='relu', name="3_dense_8")(x)
    outputs = tf.keras.layers.Dense(y_train.shape[1], name="output_layer")(x)

    model = tf.keras.Model(inputs=inputs, outputs=outputs, name="animator_model")

    # Compile the model
    model.compile(optimizer='adam', loss='mean_squared_error')

    # Train the model
    model.fit(X_train, y_train, validation_data=(X_test, y_test), epochs=70, batch_size=32)

    # Save the scalers
    with open(f"input_scaler_{model_name}.json", "w") as f:
        json.dump({"mean": scaler_X.mean_.tolist(), "std": scaler_X.scale_.tolist()}, f)

    with open(f"output_scaler_{model_name}.json", "w") as f:
        json.dump({"mean": scaler_y.mean_.tolist(), "std": scaler_y.scale_.tolist()}, f)

    # Convert to ONNX
    spec = (tf.TensorSpec((None, X_train.shape[1]), tf.float32),)
    onnx_model, _ = tf2onnx.convert.from_keras(model, input_signature=spec, opset=13)
    with open(f"{model_name}.onnx", "wb") as f:
        f.write(onnx_model.SerializeToString())

    print(f"Model saved as '{model_name}.keras' and '{model_name}.onnx'.")


if __name__ == "__main__":
    # Load the CSV data
    data = pd.read_csv('merged.csv')

    # Display the first few rows to check the structure
    print(data.head())

    # Define sensor columns (assuming they start with 'Sensor')
    sensor_columns = [col for col in data.columns if col.startswith('Sensor')]
    print("Sensor columns:", sensor_columns)

    # Define the animator parameter columns
    animator_columns = [
        "PinchBlend",
        "ThumbBlend1",
        "ThumbBlend2",
        "IndexBlend1",
        "IndexBlend2",
        "MiddleRingLittleBlend1",
        "MiddleRingLittleBlend2"
    ]
    print("Animator columns:", animator_columns)

    # Extract sensor data (inputs) and animator parameters (outputs)
    X = data[sensor_columns]
    y = data[animator_columns]

    # Scale the data for consistent neural network input and output ranges
    scaler_X = StandardScaler()
    scaler_y = StandardScaler()

    X_scaled = scaler_X.fit_transform(X)
    y_scaled = scaler_y.fit_transform(y)

    # Split into training and test sets
    X_train, X_test, y_train, y_test = train_test_split(X_scaled, y_scaled, test_size=0.2, random_state=42)

    # Create, train, and save the neural network
    create_network(X_train, y_train, X_test, y_test, scaler_X, scaler_y, sensor_columns, "888_r1023_nm_v3")

    print("Training complete.")



