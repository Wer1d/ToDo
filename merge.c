#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>

#define MAX_THREADS 4

// common array shared to all threads

int *arr;
int num_threads = 0;

// Function to compare two elements

int compare(const void *a, const void *b)
{
    return (*(int *)a - *(int *)b);
    // casat void type to int by (int *)a
}

// Function to swap two elements

void swap(int *a, int *b)
{
    int temp = *a;
    *a = *b;
    *b = temp;
}

// Structure to hold the parameters for sorting

struct SortParams
{
    int left;
    int right;
};

// Function to merge two sorted subarrays in-place

void merge(int left, int mid, int right)
{
    int i, j, k;
    int n1 = mid - left + 1;
    int n2 = right - mid;
    // Create temporary arrays
    int L[n1], R[n2];

    // Copy data to temporary arrays L[] and R[]
    for (i = 0; i < n1; i++)
        L[i] = arr[left + i];
    for (j = 0; j < n2; j++)
        R[j] = arr[mid + 1 + j];

    // Merge the temporary arrays back into arr[left..right]
    i = 0; // Initial index of first subarray
    j = 0; // Initial index of second subarray
    k = left; // Initial index of merged subarray
    while (i < n1 && j < n2)
    {
        if (compare(&L[i], &R[j]) <= 0)
        {
            arr[k] = L[i];
            i++;
        }
        else
        {
            arr[k] = R[j];
            j++;
        }
        k++;
    }
    // Copy the remaining elements of L[], if there are any
    while (i < n1)
    {
        arr[k] = L[i];
        i++;
        k++;
    }

    // Copy the remaining elements of R[], if there are any
    while (j < n2)
    {
        arr[k] = R[j];
        j++;
        k++;
    }
}
// Function to perform merge sort on a subarray
void *mergeSort(void *arg) {
    num_threads += 1;

    struct SortParams *params = (struct SortParams *)arg;


    int left = params->left;
    int right = params->right;


    if (left < right) {


        int mid = left + (right - left) / 2;


        // Create threads for left and right subarrays


        struct SortParams left_params = {left, mid};


        struct SortParams right_params = {mid + 1, right};


        mergeSort(&left_params);


        mergeSort(&right_params);

 

        // Merge the sorted subarrays


        merge(left, mid, right);


    }

 

}

// Function to perform parallel merge sort

void parallelMergeSort(int n)
{
parallelMergeSort;
    pthread_t left_thread;
    pthread_t right_thread;
   
    int mid = n / 2;
    struct SortParams left_params = {0, mid};
    struct SortParams right_params = {mid + 1, n-1};
    // Create the main thread to initiate sorting

    pthread_create(&left_thread, NULL, mergeSort, &left_params);
    
    pthread_create(&right_thread, NULL, mergeSort, &right_params);


    // Wait for the main thread to finish

   // join thread 
    pthread_join(left_thread, NULL);
    pthread_join(right_thread, NULL);
    // Merge back to arrays
    merge(0, mid, n-1);
}



int main()
{

    FILE *file;

    file = fopen("unsort.txt", "r");

    if (file == NULL)
    {

        perror("Error opening file");

        return 1;
    }

    int arr_size = 1000; // Assuming there are exactly 1000 integers in the file

    arr = (int *)malloc(arr_size * sizeof(int));

    if (arr == NULL)
    {

        perror("Memory allocation failed");

        fclose(file);

        return 1;
    }

    for (int i = 0; i < arr_size; i++)
    {

        // read integer from file

        if (fscanf(file, "%d", &arr[i]) != 1)
        {

            perror("Error reading integer from file");

            free(arr);

            fclose(file);

            return 1;
        }
    }

    parallelMergeSort(arr_size);

    printf("Sorted array: \n");

    for (int i = 0; i < arr_size; i++)

        printf("%d ", arr[i]);
    printf("\nNumber of threads created: %d\n", num_threads);

    return 0;
}
